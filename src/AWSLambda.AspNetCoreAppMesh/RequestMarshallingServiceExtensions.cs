using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AWSLambda.AspNetCoreAppMesh
{
    public static class RequestMarshallingServiceExtensions
    {
        public static IServiceProvider Services { get; set; }

        public static Task<APIGatewayProxyResponse> RouteAPIGatewayProxyRequestLocally(this InvokeRequest invokeRequest, CancellationToken cancellationToken = default)
        {
            var marshallingSvc = Services.GetService<IRequestMarshallingService>();

            return marshallingSvc.MarshallAPIGatewayProxyRequest(invokeRequest, cancellationToken);
        }

        public static Task<ApplicationLoadBalancerResponse> RouteApplicationLoadBalancerRequestLocally(this InvokeRequest invokeRequest, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
