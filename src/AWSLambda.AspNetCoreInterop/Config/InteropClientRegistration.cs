using Amazon.Lambda.AspNetCoreServer;
using AWSLambda.AspNetCoreInterop;
using AWSLambda.AspNetCoreInterop.Config;
using AWSLambda.AspNetCoreInterop.Util;
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
    public static class LambdaInteropClientService
    {
        public static void AddAWSLambdaInteropClient(this IServiceCollection services, Action<LambdaInteropOptions> config)
        {
            services.Configure(config);

            services.AddSingleton<IApplicationUrlResolver, FromLaunchSettingsApplicationUrlResolver>();

            services.AddHttpClient<IFunctionRegistryClient, FunctionRegistryClient>();

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
    public static class LambdaInteropClientAppFeature
    {
        static LambdaInteropOptions GetInteropOptions(IServiceProvider services)
        {
            var iopts = (IOptions<LambdaInteropOptions>)services.GetService(typeof(IOptions<LambdaInteropOptions>));

            if (iopts == null)
                throw new InteropException($"Ensure AddAWSLambdaInteropClient() has been called in ConfigureServices() method of your Startup.");

            return iopts.Value;
        }

        public static IApplicationBuilder UseAWSLambdaInteropClient(this IApplicationBuilder app)
        {
            var opts = GetInteropOptions(app.ApplicationServices);

            if (string.IsNullOrEmpty(opts.LambdaName))
                throw new InteropException($"Ensure {opts.LambdaName} is set in {nameof(LambdaInteropOptions)}");

            if (string.IsNullOrEmpty(opts.RegistryUrl))
                throw new InteropException($"Ensure {opts.RegistryUrl} is set in {nameof(LambdaInteropOptions)}");

            RequestMarshallingServiceExtensions.Services = app.ApplicationServices;

            return app;
        }

        public static IApplicationBuilder HandleIncomingAWSLambdaInvokeRequests(this IApplicationBuilder app, IHostingEnvironment env)
        {
            var opts = GetInteropOptions(app.ApplicationServices);

            // safeguard
            if (!env.IsDevelopment() && opts.HandleIncomingRequestsInDevelopmentOnly == true)
                throw new InteropException($"Handling of incoming requests is allowed in Development environment only. Consider setting {nameof(opts.HandleIncomingRequestsInDevelopmentOnly)} option to false.");

            ValidateInteropOptions(opts, app.ApplicationServices);

            var logger = (ILogger<HandleIncomingInvokeRequestsMiddleware>)app.ApplicationServices.GetService(typeof(ILogger<HandleIncomingInvokeRequestsMiddleware>));

            app.MapWhen(context => context.Request.Path.StartsWithSegments(opts.HandlerPathForIncomingRequests), appBuilder => appBuilder.UseMiddleware<HandleIncomingInvokeRequestsMiddleware>());

            var funcRegistryClient = (IFunctionRegistryClient)app.ApplicationServices.GetService(typeof(IFunctionRegistryClient));

            funcRegistryClient.RegisterFunction().GetAwaiter().GetResult();

            var listenUrl = UriUtil.Combine(opts.ApplicationUrl, opts.HandlerPathForIncomingRequests);

            logger.LogInformation($"Registered with registry {opts.RegistryUrl}. Listening for incoming requests on ${listenUrl}");

            return app;
        }

        static void ValidateInteropOptions(LambdaInteropOptions opts, IServiceProvider services)
        {
            if (string.IsNullOrEmpty(opts.ApplicationUrl))
            {
                var appUrlResolver = (IApplicationUrlResolver)services.GetService(typeof(IApplicationUrlResolver));

                opts.ApplicationUrl = appUrlResolver?.GetApplicationUrl();
            }

            var props = opts.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var p in props)
            {
                var v = p.GetValue(opts);

                if (v is string)
                {
                    var strVal = (string)v;

                    if (string.IsNullOrEmpty(strVal))
                    {
                        throw new InteropException($"Ensure {p.Name} is set in {nameof(LambdaInteropOptions)}");
                    }
                }
            }
        }
    }
}
