using Amazon.Lambda.APIGatewayEvents;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public static class APIGatewayProxyRequestRouter
    {
        public static LocalRouterClient RouterClient { get; set; }
        
        public static async Task RouteLocally(this APIGatewayProxyRequest request, string destinationLambdaName, CancellationToken cancellationToken)
        {
            await RouterClient.RouteRequest(destinationLambdaName, request, cancellationToken);
        }
    }
}
