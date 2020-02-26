using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Config
{
    public interface ILambdaInteropOptions
    {
        string LambdaName { get; set; }
        string RouterUrl { get; set; }
        string APIGatewayProxyRequestHandlerPath { get; set; }
        string ApplicationLoadBalancerRequestHandlerPath { get; set; }
    }

    public class LambdaInteropOptions : ILambdaInteropOptions
    {
        public string LambdaName { get; set; }
        public string RouterUrl { get; set; }
        public string APIGatewayProxyRequestHandlerPath { get; set; } = "/lambda-interop-handler-api-gateway";
        public string ApplicationLoadBalancerRequestHandlerPath { get; set; } = "/lambda-interop-handler-alb";
    }
}
