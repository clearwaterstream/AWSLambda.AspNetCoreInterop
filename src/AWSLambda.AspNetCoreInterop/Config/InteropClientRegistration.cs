using Amazon.Lambda.AspNetCoreServer;
using AWSLambda.AspNetCoreInterop;
using AWSLambda.AspNetCoreInterop.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LambdaInteropClientService
    {
        public static void AddAWSLambdaInteropClient(this IServiceCollection services, Action<LambdaInteropOptions> config)
        {
            services.Configure(config);
            
            services.AddHttpClient<IRouterClientService, RouterClientService>();
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

            RouterClientServiceExtensions.Services = app.ApplicationServices;

            return app;
        }

        public static IApplicationBuilder HandleIncomingAPIGatewayProxyRequests<TEntryPoint>(this IApplicationBuilder app, IHostingEnvironment env) where TEntryPoint : APIGatewayProxyFunction
        {
            var opts = GetInteropOptions(app.ApplicationServices);

            // safeguard
            if (!env.IsDevelopment() && opts.HandleIncomingRequestsInDevelopmentOnly == true)
            {
                throw new InteropException($"Handling of incoming requests is allowed in Development environment only. Consider setting {nameof(opts.HandleIncomingRequestsInDevelopmentOnly)} option to false.");
            }

            var logger = (ILogger<ProxiedRequestHandlerMiddleware>)app.ApplicationServices.GetService(typeof(ILogger<ProxiedRequestHandlerMiddleware>));

            app.MapWhen(context => context.Request.Path.StartsWithSegments(opts.HandlerPathForIncomingRequests), appBuilder => appBuilder.UseMiddleware<ProxiedRequestHandlerMiddleware>());

            var routerSvc = (IRouterClientService)app.ApplicationServices.GetService(typeof(IRouterClientService));

            routerSvc.RegisterWithRouter().GetAwaiter().GetResult();

            logger.LogInformation($"Registered with router {opts.RouterUrl}. Listening for incoming requests on ${opts.HandlerPathForIncomingRequests}");

            return app;
        }
    }
}
