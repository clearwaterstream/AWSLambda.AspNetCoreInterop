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
        string DeploymentType { get; set; }
        string ApplicationUrl { get; set; }
        string HandlerPathForIncomingRequests { get; set; }
        bool HandleIncomingRequestsInDevelopmentOnly { get; set; }
    }

    public class LambdaInteropOptions : ILambdaInteropOptions
    {
        public string LambdaName { get; set; }
        public string RouterUrl { get; set; }
        public string DeploymentType { get; set; } = Config.DeploymentType.APIGateway;
        public string ApplicationUrl { get; set; }
        public string HandlerPathForIncomingRequests { get; set; } = "/lambda-interop-handler";
        public bool HandleIncomingRequestsInDevelopmentOnly { get; set; } = true;
    }
}
