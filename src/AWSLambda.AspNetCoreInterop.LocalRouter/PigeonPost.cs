using Amazon.Lambda.APIGatewayEvents;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    // route requests
    public class PigeonPost : Hub
    {
        readonly ILogger<PigeonPost> logger = null;

        public PigeonPost(ILogger<PigeonPost> logger)
        {
            this.logger = logger;
        }

        public async Task RegisterLambda(string lambdaName)
        {
            var groupName = lambdaName.ToLowerInvariant();

            logger.LogInformation($"{Context.UserIdentifier} registered to group {groupName}");
            
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
        
        public Task RouteApiGatewayRequest(string destinationLambdaName, APIGatewayProxyRequest request)
        {
            var groupName = destinationLambdaName.ToLowerInvariant();

            return Clients.Group(groupName).SendAsync("receive-api-gateway-request", request);
        }
    }
}
