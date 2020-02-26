using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.Lambda.TestUtilities;
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

            var routerUrl = configuration["aws-lambda-interop:router-host"];

            if(string.IsNullOrEmpty(routerUrl))
            {
                throw new InteropClientException("Ensure aws-lambda-interop: { router-host: \"http(s)://router_addr:port\" } is set in appsettings.json");
            }

            logger.LogInformation($"router hub endpoint is {routerUrl}");
        }

        public async Task RouteRequest(string destinationLambdaName, APIGatewayProxyRequest request, CancellationToken cancellationToken)
        {
            await hubConnection.InvokeAsync("RouteApiGatewayRequest", destinationLambdaName, request, cancellationToken);
        }

        async Task HandleInvoke(InvokeRequest invokeRequest)
        {
            try
            {
                var lambdaContext = new TestLambdaContext();

                // todo
            }
            catch (Exception ex)
            {
                // todo - more infromative logging
                logger.LogError(ex, $"Error invoking lambda handler for incoming {nameof(APIGatewayProxyRequest)}");
            }
        }

        async Task HandleIncomingRequest(APIGatewayProxyRequest request, ILambdaContext lambdaContext)
        {
            await lambdaEntryPoint.FunctionHandlerAsync(request, lambdaContext);
        }

        async Task HandleIncomingRequest(ApplicationLoadBalancerRequest request, ILambdaContext lambdaContext)
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
            }
        }
    }
}
