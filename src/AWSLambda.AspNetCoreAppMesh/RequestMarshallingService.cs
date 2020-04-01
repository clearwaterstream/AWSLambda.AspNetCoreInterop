using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Model;
using AWSLambda.AspNetCoreAppMesh.Config;
using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh
{
    public interface IRequestMarshallingService
    {
        Task<APIGatewayProxyResponse> MarshallAPIGatewayProxyRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken);
        Task<ApplicationLoadBalancerResponse> MarshallApplicationLoadBalancerRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken);
    }

    public class RequestMarshallingService : IRequestMarshallingService
    {
        readonly HttpClient httpClient;
        readonly ILogger<RequestMarshallingService> logger;
        readonly LambdaAppMeshOptions appMeshOptions;
        readonly ICatalogRegistrarAgent catalogAgent;

        public RequestMarshallingService(HttpClient httpClient, ILogger<RequestMarshallingService> logger, IOptions<LambdaAppMeshOptions> opts, ICatalogRegistrarAgent catalogAgent)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            appMeshOptions = opts.Value;
            this.catalogAgent = catalogAgent;
        }

        public async Task<APIGatewayProxyResponse> MarshallAPIGatewayProxyRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            var resp = await Invoke<APIGatewayProxyResponse>(invokeRequest, "APIGatewayProxyRequest", cancellationToken);

            if (resp.payload == null)
            {
                return new APIGatewayProxyResponse()
                {
                    StatusCode = resp.statusCode
                };
            }

            return resp.payload;
        }


        public Task<ApplicationLoadBalancerResponse> MarshallApplicationLoadBalancerRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<(int statusCode, TResp payload)> Invoke<TResp>(InvokeRequest invokeRequest, string payloadType, CancellationToken cancellationToken)
        {
            var destLambdaOpts = await catalogAgent.GetFunctionInfo(invokeRequest.FunctionName);

            var invokeHandlerUrl = UriUtil.Combine(destLambdaOpts.ApplicationUrl, destLambdaOpts.HandlerPathForIncomingRequests);

            var url = $"{invokeHandlerUrl}?lambdaName={invokeRequest.FunctionName}&invocationType={invokeRequest.InvocationType}&payloadType={payloadType}&source={appMeshOptions.LambdaName}";

            using var reqMsg = new HttpRequestMessage(HttpMethod.Post, url);
            
            using (reqMsg.Content = CreateContent(invokeRequest))
            {
                using var resp = await httpClient.SendAsync(reqMsg, cancellationToken);
                
                if (!resp.IsSuccessStatusCode)
                {
                    return ((int)resp.StatusCode, default);
                }

                using var respStream = await resp.Content.ReadAsStreamAsync();

                var deserialized = await JsonSerializer.DeserializeAsync<TResp>(respStream, cancellationToken: cancellationToken);

                return ((int)resp.StatusCode, deserialized);
            }
        }

        static HttpContent CreateContent(InvokeRequest invokeRequest)
        {
            if (invokeRequest.PayloadStream != null)
                return new StreamContent(invokeRequest.PayloadStream);

            return new StringContent(invokeRequest.Payload);
        }
    }
}
