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
    public interface IRouterClientService
    {
        Task<APIGatewayProxyResponse> InvokeAPIGatewayProxyRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken);
        Task<ApplicationLoadBalancerResponse> InvokeApplicationLoadBalancerRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken);
        Task RegisterWithRouter();
    }

    public class RouterClientService : IRouterClientService
    {
        readonly HttpClient httpClient;
        readonly ILogger<RouterClientService> logger;
        readonly LambdaInteropOptions interopOptions;
        readonly string proxyRequestUrl;

        public RouterClientService(HttpClient httpClient, ILogger<RouterClientService> logger, IOptions<LambdaInteropOptions> opts)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            interopOptions = opts.Value;

            proxyRequestUrl = UriUtil.Combine(interopOptions.RouterUrl, "proxy-request");
        }

        public async Task<APIGatewayProxyResponse> InvokeAPIGatewayProxyRequest(InvokeRequest invokeRequest, CancellationToken cancellationToken)
        {
            var resp = await Invoke(invokeRequest, nameof(APIGatewayProxyRequest), (s) => JsonUtil.Deserialize<APIGatewayProxyResponse>(s), cancellationToken);

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
            var url = $"{proxyRequestUrl}?invocationType={invokeRequest.InvocationType}&payloadType={payloadType}&source={interopOptions.LambdaName}";

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

        public async Task RegisterWithRouter()
        {
            var url = UriUtil.Combine(interopOptions.RouterUrl, "register");

            using (var reqMsg = new HttpRequestMessage(HttpMethod.Post, url))
            {
                using (var ms = new MemoryStream())
                {
                    using (reqMsg.Content = new StreamContent(ms))
                    {
                        reqMsg.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        
                        JsonUtil.SerializeAndLeaveOpen(ms, interopOptions);

                        ms.Position = 0;

                        using (var resp = await httpClient.SendAsync(reqMsg))
                        {
                            if (!resp.IsSuccessStatusCode)
                            {
                                throw new InteropException($"Error registering with router ${interopOptions.RouterUrl}. Status code {resp.StatusCode}. Ensure the router is running and is accessible.");
                            }
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
