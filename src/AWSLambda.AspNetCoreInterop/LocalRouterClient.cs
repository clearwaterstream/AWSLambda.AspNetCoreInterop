using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.TestUtilities;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public class LocalRouterClient : IDisposable
    {
        readonly HubConnection hubConnection = null;
        readonly ILogger<LocalRouterClient> logger = null;
        APIGatewayProxyFunction lambdaEntryPoint = null;
        readonly IServiceProvider services = null;
        readonly string lambdaName;
        readonly Type lambdaEntryPointType;

        public LocalRouterClient(IServiceProvider services, string lambdaName, Type lambdaEntryPointType)
        {
            logger = (ILogger<LocalRouterClient>)services.GetService(typeof(ILogger<LocalRouterClient>));

            this.lambdaName = lambdaName;
            this.lambdaEntryPointType = lambdaEntryPointType;
            
            this.services = services;

            var configuration = (IConfiguration)services.GetService(typeof(IConfiguration));

            var routerHostAddr = configuration["aws-lambda-interop:router-host"];

            if(string.IsNullOrEmpty(routerHostAddr))
            {
                throw new InteropClientException("Ensure aws-lambda-interop: { router-host: \"http(s)://router_addr:port\" } is set in appsettings.json");
            }

            var url = UriUtil.Combine(routerHostAddr, "request-routing-hub");

            logger.LogInformation($"router hub endpoint is {url}");

            hubConnection = new HubConnectionBuilder()
                .WithUrl(routerHostAddr)
                .Build();

            hubConnection.On<APIGatewayProxyRequest>("receive-api-gateway-request", async (req) =>
            {
                await HandleIncomingRequest(req);
            });

            hubConnection.On<ApplicationLoadBalancerRequest>("receive-alb-request", async (req) =>
            {
                await HandleIncomingRequest(req);
            });
        }

        public async Task RouteRequest(string destinationLambdaName, APIGatewayProxyRequest request, CancellationToken cancellationToken)
        {
            await hubConnection.InvokeAsync("RouteApiGatewayRequest", destinationLambdaName, request, cancellationToken);
        }

        public async Task Connect()
        {
            lambdaEntryPoint = (APIGatewayProxyFunction)services.GetService(lambdaEntryPointType);
            
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


        async Task HandleIncomingRequest(ApplicationLoadBalancerRequest request)
        {
            await Task.CompletedTask;
            
            throw new NotImplementedException();
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
