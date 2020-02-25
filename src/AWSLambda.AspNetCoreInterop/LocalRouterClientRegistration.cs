using Amazon.Lambda.AspNetCoreServer;
using AWSLambda.AspNetCoreInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LocalRouterClientRegistration
    {
        public static void AddAWSLambdaInteropClient<TLambdaEntryPoint>(this IServiceCollection services, string lambdaName) where TLambdaEntryPoint: APIGatewayProxyFunction
        {
            services.AddSingleton(async (svcProvider) =>
            {
                var client = new LocalRouterClient(svcProvider, lambdaName, typeof(TLambdaEntryPoint));

                APIGatewayProxyRequestRouter.RouterClient = client;

                await client.Connect();

                return client;
            });
        }
    }
}
