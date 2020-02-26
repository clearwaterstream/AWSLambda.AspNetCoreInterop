using Amazon.Lambda.AspNetCoreServer;
using AWSLambda.AspNetCoreInterop;
using AWSLambda.AspNetCoreInterop.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LambdaInteropClientService
    {
        public static void AddAWSLambdaInteropClient<TLambdaEntryPoint>(this IServiceCollection services, Action<LambdaInteropOptions> config) where TLambdaEntryPoint: APIGatewayProxyFunction
        {
            services.Configure(config);
            
            services.AddHttpClient<IRouterHttpClient, RouterHttpClient>();

            services.AddSingleton<ILambdaInvokeMarshaller, LambdaInvokeMarshaller>();
        }
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    public static class LambdaInteropClientAppFeature
    {
        public static IApplicationBuilder UseAWSLambdaInteropClient<TEntryPoint>(this IApplicationBuilder app)
        {
            LambdaInvokeMarshallerExtensions.Services = app.ApplicationServices;

            return app;
        }

        public static IApplicationBuilder HandleIncomingLambdaInvokeRequests<TEntryPoint>(this IApplicationBuilder app) where TEntryPoint : APIGatewayProxyFunction
        {
            var iopts = (IOptions<LambdaInteropOptions>)app.ApplicationServices.GetService(typeof(IOptions<LambdaInteropOptions>));

            var opts = iopts.Value;

            app.MapWhen(context => context.Request.Path.StartsWithSegments(opts.APIGatewayProxyRequestHandlerPath), appBuilder => appBuilder.UseMiddleware<ProxiedLambdaRequestHandlerMiddleware>());

            return app;
        }
    }
}
