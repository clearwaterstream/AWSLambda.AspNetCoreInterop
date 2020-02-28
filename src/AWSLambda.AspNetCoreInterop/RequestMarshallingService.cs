using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.ApplicationLoadBalancerEvents;
using Amazon.Lambda.Model;
using AWSLambda.AspNetCoreInterop.Config;
using AWSLambda.AspNetCoreInterop.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public interface IRequestMarshallingService
    {
        Task<APIGatewayProxyResponse> InvokeAPIGatewayProxyRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken);
        Task<ApplicationLoadBalancerResponse> InvokeApplicationLoadBalancerRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken);
    }

    public class RequestMarshallingService : IRequestMarshallingService
    {
        readonly HttpClient httpClient;
        readonly ILogger<RequestMarshallingService> logger;
        readonly LambdaInteropOptions interopOptions;
        readonly IFunctionRegistryClient functionRegistryClient;

        public RequestMarshallingService(HttpClient httpClient, ILogger<RequestMarshallingService> logger, IOptions<LambdaInteropOptions> opts, IFunctionRegistryClient functionRegistryClient)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            interopOptions = opts.Value;
            this.functionRegistryClient = functionRegistryClient;
        }

        public async Task<APIGatewayProxyResponse> InvokeAPIGatewayProxyRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            var resp = await Invoke(invokeRequest, "APIGatewayProxyRequest", (s) => JsonUtil.Deserialize<APIGatewayProxyResponse>(s), cancellationToken);

            if (resp.payload == null)
            {
                return new APIGatewayProxyResponse()
                {
                    StatusCode = resp.statusCode
                };
            }

            return resp.payload;
        }


        public Task<ApplicationLoadBalancerResponse> InvokeApplicationLoadBalancerRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        async Task<(int statusCode, TResp payload)> Invoke<TResp>(InvokeRequest invokeRequest, string payloadType, Func<Stream, TResp> deserializePayload, CancellationToken cancellationToken)
        {
            var destLambdaOpts = await functionRegistryClient.GetFunctionInfo(invokeRequest.FunctionName);

            var invokeHandlerUrl = UriUtil.Combine(destLambdaOpts.ApplicationUrl, destLambdaOpts.HandlerPathForIncomingRequests);

            var url = $"{invokeHandlerUrl}?lambdaName={invokeRequest.FunctionName}invocationType={invokeRequest.InvocationType}&payloadType={payloadType}&source={interopOptions.LambdaName}";

            using (var reqMsg = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (reqMsg.Content = CreateContent(invokeRequest))
                {
                    using (var resp = await httpClient.SendAsync(reqMsg, cancellationToken))
                    {
                        if (!resp.IsSuccessStatusCode)
                        {
                            return ((int)resp.StatusCode, default);
                        }

                        using (var respStream = await resp.Content.ReadAsStreamAsync())
                        {
                            var deserialized = deserializePayload(respStream);

                            return ((int)resp.StatusCode, deserialized);
                        }
                    }
                }
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
