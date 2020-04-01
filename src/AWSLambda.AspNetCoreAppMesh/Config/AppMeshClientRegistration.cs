using Amazon.Lambda.AspNetCoreServer;
using AWSLambda.AspNetCoreAppMesh;
using AWSLambda.AspNetCoreAppMesh.Config;
using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LambdaAppMeshClientService
    {
        public static void AddAWSLambdaAppMeshClient(this IServiceCollection services, Action<LambdaAppMeshOptions> config)
        {
            services.Configure(config);

            services.AddSingleton<IPairingTokenResolver, PairingTokenResolver>();

            services.AddSingleton<IApplicationUrlResolver, KestrelApplicationUrlResolver>();

            services.AddHttpClient<ICatalogRegistrarAgent, CatalogRegistrarAgent>();

            services.AddHttpClient<IRequestMarshallingService, RequestMarshallingService>();
        }

        public static void AddAPIGatewayProxyFunctionEntryPoint<TEntryPoint>(this IServiceCollection services) where TEntryPoint : APIGatewayProxyFunction, new()
        {
            services.AddSingleton<IAPIGatewayProxyFunctionActivator, APIGatewayProxyFunctionActivator<TEntryPoint>>();
        }

        public static void AddApplicationLoadBalancerFunctionEntryPoint<TEntryPoint>(this IServiceCollection services) where TEntryPoint : ApplicationLoadBalancerFunction, new()
        {
            services.AddSingleton<IApplicationLoadBalancerFunctionActivator, ApplicationLoadBalancerFunctionActivator<TEntryPoint>>();
        }
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    using Microsoft.AspNetCore.Hosting.Server;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public static class LambdaAppMeshClientAppFeature
    {
        static LambdaAppMeshOptions GetAppMeshOptions(IServiceProvider services)
        {
            var iopts = services.GetService<IOptions<LambdaAppMeshOptions>>();

            if (iopts == null)
                throw new AppMeshException($"Ensure {nameof(LambdaAppMeshClientService.AddAWSLambdaAppMeshClient)} has been called in ConfigureServices() method of your Startup.");

            return iopts.Value;
        }

        public static IApplicationBuilder UseAWSLambdaAppMeshClient(this IApplicationBuilder app)
        {
            var opts = GetAppMeshOptions(app.ApplicationServices);

            if (string.IsNullOrEmpty(opts.LambdaName))
                throw new AppMeshException($"Ensure {opts.LambdaName} is set in {nameof(LambdaAppMeshOptions)}");

            if (string.IsNullOrEmpty(opts.CatalogUrl))
                throw new AppMeshException($"Ensure {opts.CatalogUrl} is set in {nameof(LambdaAppMeshOptions)}");

            RequestMarshallingServiceExtensions.Services = app.ApplicationServices;

            return app;
        }

        public static IApplicationBuilder HandleIncomingAWSLambdaInvokeRequests(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            var server = app.ApplicationServices.GetService<IServer>();

            if(server.GetType().Name == "LambdaServer")
            {
                // Inception -- we are 2nd level deep here. Wake up.
                
                return app;
            }

            var opts = GetAppMeshOptions(app.ApplicationServices);

            // safeguard
            if (!env.IsDevelopment() && opts.HandleIncomingRequestsInDevelopmentOnly == true)
                throw new AppMeshException($"Handling of incoming requests is allowed in Development environment only. Consider setting {nameof(opts.HandleIncomingRequestsInDevelopmentOnly)} option to false.");

            ValidateAppMeshOptions(opts, app.ApplicationServices);

            var logger = app.ApplicationServices.GetService<ILogger<HandleIncomingInvokeRequestsMiddleware>>();

            app.MapWhen(context => context.Request.Path.StartsWithSegments(opts.HandlerPathForIncomingRequests), appBuilder => appBuilder.UseMiddleware<HandleIncomingInvokeRequestsMiddleware>());

            var catalogAgent = app.ApplicationServices.GetService<ICatalogRegistrarAgent>();

            catalogAgent.RegisterFunction().GetAwaiter().GetResult();

            var listenUrl = UriUtil.Combine(opts.ApplicationUrl, opts.HandlerPathForIncomingRequests);

            logger.LogInformation($"Registered with catalog {opts.CatalogUrl}. Listening for incoming requests on {listenUrl}");

            return app;
        }

        static void ValidateAppMeshOptions(LambdaAppMeshOptions opts, IServiceProvider services)
        {
            if (string.IsNullOrEmpty(opts.ApplicationUrl))
            {
                var appUrlResolver = services.GetService<IApplicationUrlResolver>();

                opts.ApplicationUrl = appUrlResolver?.GetApplicationUrl();
            }

            var props = opts.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                var v = p.GetValue(opts);

                if (p.PropertyType == typeof(string))
                {
                    var strVal = (string)v;

                    if (string.IsNullOrEmpty(strVal))
                    {
                        throw new AppMeshException($"Ensure {p.Name} is set in {nameof(LambdaAppMeshOptions)}");
                    }
                }
            }
        }
    }
}
