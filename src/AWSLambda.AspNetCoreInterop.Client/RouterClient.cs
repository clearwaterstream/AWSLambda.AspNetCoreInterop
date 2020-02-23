using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.TestUtilities;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.Client
{
    public class RouterClient : IDisposable
    {
        readonly HubConnection _hubConnection = null;
        readonly string _lambdaName = null;
        readonly APIGatewayProxyFunction _entryPoint = null;

        public RouterClient(string lambdaName, APIGatewayProxyFunction entryPoint)
        {
            _lambdaName = lambdaName;
            _entryPoint = entryPoint;

            var url = "http://127.0.0.1:5000/pigeon-post";

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(url)
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<APIGatewayProxyRequest>("receive-api-gateway-request", HandleIncomingRequest);
        }

        public async Task Connect()
        {
            await _hubConnection.StartAsync();

            await _hubConnection.InvokeAsync("RegisterLambda", _lambdaName);
        }

        async Task HandleIncomingRequest(APIGatewayProxyRequest request)
        {
            var lambdaContext = new TestLambdaContext();
            // todo -- fill out the context to the extend possible

            await _entryPoint.FunctionHandlerAsync(request, lambdaContext);
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
                _hubConnection?.DisposeAsync().GetAwaiter().GetResult();
            }
        }
    }
}
