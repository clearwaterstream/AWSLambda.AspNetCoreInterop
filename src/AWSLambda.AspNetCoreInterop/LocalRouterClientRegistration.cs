using Amazon.Lambda.AspNetCoreServer;
using AWSLambda.AspNetCoreInterop;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LambdaInteropClientService
    {
        public static void AddAWSLambdaInteropClient<TLambdaEntryPoint>(this IServiceCollection services, string lambdaName) where TLambdaEntryPoint: APIGatewayProxyFunction
        {
            services.AddSingleton(svcProvider =>
            {
                var client = new LocalRouterClient(svcProvider, lambdaName, typeof(TLambdaEntryPoint));

                APIGatewayProxyRequestRouter.RouterClient = client;

                return client;
            });
        }
    }
}

namespace Microsoft.AspNetCore.Hosting
{
    public static class LambdaInteropClientAppFeature
    {
        public static T UseAWSLambdaInteropClient<T>(this T builder) where T : IWebHostBuilder
        {
            var urls = builder.GetSetting(WebHostDefaults.ServerUrlsKey);

            var urlSeparator = ';';

            var url = urls.Split(urlSeparator);

            return builder;
        }
    }
}
