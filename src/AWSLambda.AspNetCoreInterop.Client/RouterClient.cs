using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.TestUtilities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.Client
{
    public class RouterClient : IDisposable
    {
        readonly HubConnection hubConnection = null;
        readonly ILogger<RouterClient> logger = null;
        APIGatewayProxyFunction lambdaEntryPoint = null;
        readonly IServiceProvider services = null;

        public RouterClient(ILogger<RouterClient> logger, IConfiguration configuration, IServiceProvider services)
        {
            this.logger = logger;
            this.services = services;
            
            var routerHostAddr = configuration["aws-lambda-interop:router-host"];

            if(string.IsNullOrEmpty(routerHostAddr))
            {
                throw new InteropClientException("Ensure aws-lambda-interop: { router-host: \"http(s)://router_addr:port\" } is set in appsettings.json");
            }

            var url = UriUtil.Combine(routerHostAddr, "pigeon-post");

            this.logger.LogInformation($"router hub endpoint is {url}");

            hubConnection = new HubConnectionBuilder()
                .WithUrl(routerHostAddr)
                .WithAutomaticReconnect()
                .Build();

            hubConnection.On<APIGatewayProxyRequest>("receive-api-gateway-request", HandleIncomingRequest);
        }

        public async Task RegisterWithRouter<TLambdaEntryPoint>(string lambdaName) where TLambdaEntryPoint: APIGatewayProxyFunction
        {
            lambdaEntryPoint = (APIGatewayProxyFunction)services.GetService(typeof(TLambdaEntryPoint));
            
            await hubConnection.StartAsync();

            await hubConnection.InvokeAsync("RegisterLambda", lambdaName);
        }

        async Task HandleIncomingRequest(APIGatewayProxyRequest request)
        {
            try
            {
                var lambdaContext = new TestLambdaContext();
                // todo -- fill out the context to the extend possible

                await lambdaEntryPoint.FunctionHandlerAsync(request, lambdaContext);
            }
            catch(Exception ex)
            {
                // todo - more infromative logging
                logger.LogError(ex, $"Error invoking lambda handler for incoming {nameof(APIGatewayProxyRequest)}");
            }
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                hubConnection?.DisposeAsync().GetAwaiter().GetResult();
            }
        }
    }
}
