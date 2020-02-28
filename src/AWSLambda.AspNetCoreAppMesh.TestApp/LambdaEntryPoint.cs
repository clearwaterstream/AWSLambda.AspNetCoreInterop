using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.AspNetCoreServer.Internal;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.TestApp
{
    public class LambdaEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>();
        }

        protected override void MarshallRequest(InvokeFeatures features, APIGatewayProxyRequest apiGatewayRequest, ILambdaContext lambdaContext)
        {
            base.MarshallRequest(features, apiGatewayRequest, lambdaContext);
        }

        protected override void PostMarshallConnectionFeature(IHttpConnectionFeature aspNetCoreConnectionFeature, APIGatewayProxyRequest lambdaRequest, ILambdaContext lambdaContext)
        {
            base.PostMarshallConnectionFeature(aspNetCoreConnectionFeature, lambdaRequest, lambdaContext);
        }

        protected override void PostMarshallRequestFeature(IHttpRequestFeature aspNetCoreRequestFeature, APIGatewayProxyRequest lambdaRequest, ILambdaContext lambdaContext)
        {
            base.PostMarshallRequestFeature(aspNetCoreRequestFeature, lambdaRequest, lambdaContext);
        }
    }
}
