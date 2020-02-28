using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public static class RequestMarshallingServiceExtensions
    {
        public static IServiceProvider Services { get; set; }

        public static Task<APIGatewayProxyResponse> InvokeAPIGatewayProxyRequestLocally(this InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            var marshallingSvc = (IRequestMarshallingService)Services.GetService(typeof(IRequestMarshallingService));

            return marshallingSvc.InvokeAPIGatewayProxyRequest(invokeRequest, cancellationToken);
        }

        public static Task<ApplicationLoadBalancerResponse> InvokeApplicationLoadBalancerRequestLocally(this InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
