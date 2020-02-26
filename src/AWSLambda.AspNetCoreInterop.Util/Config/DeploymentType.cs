using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Config
{
    public static class DeploymentType
    {
        public static readonly string APIGateway = nameof(APIGateway);
        public static readonly string ApplicationLoadBalancer = nameof(ApplicationLoadBalancer);
    }
}
