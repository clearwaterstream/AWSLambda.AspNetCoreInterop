using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AWSLambda.AspNetCoreInterop.Config;
using AWSLambda.AspNetCoreInterop.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AWSLambda.AspNetCoreInterop
{
    public interface IFunctionRegistryClient
    {
        Task RegisterFunction();
        Task<LambdaInteropOptions> GetFunctionInfo(string lambdaName);
    }

    public class FunctionRegistryClient : IFunctionRegistryClient
    {
        readonly HttpClient httpClient;
        readonly ILogger<FunctionRegistryClient> logger;
        readonly LambdaInteropOptions interopOptions;

        public FunctionRegistryClient(HttpClient httpClient, ILogger<FunctionRegistryClient> logger, IOptions<LambdaInteropOptions> opts)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            interopOptions = opts.Value;
        }

        public async Task RegisterFunction()
        {
            var url = UriUtil.Combine(interopOptions.RegistryUrl, "register");

            try
            {
                await RegisterFunctionInternal(url);
            }
            catch (InteropException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InteropException($"Error registering with registry {interopOptions.RegistryUrl}. Ensure the registry is running and is accessible.", ex);
            }
        }

        public async Task<LambdaInteropOptions> GetFunctionInfo(string lambdaName)
        {
            var methodUrl = UriUtil.Combine(interopOptions.RegistryUrl, "function-info");

            var url = $"{methodUrl}?lambdaName={lambdaName}";

            try
            {
                using(var resp = await httpClient.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                        throw new InteropException($"Function {lambdaName} is not registered with registry {interopOptions.RegistryUrl}");
                    else if (!resp.IsSuccessStatusCode)
                        throw new InteropException($"Error obtaining function information from registry {url}. Status code {resp.StatusCode}");

                    using(var stream = await resp.Content.ReadAsStreamAsync())
                    {
                        var opts = JsonUtil.Deserialize<LambdaInteropOptions>(stream);

                        return opts;
                    }
                }
            }
            catch(InteropException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new InteropException($"Error obtaining function information from registry {url}", ex);
            }
        }

        async Task RegisterFunctionInternal(string url)
        {
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
                                throw new InteropException($"Error registering with registry {interopOptions.RegistryUrl}. Status code {resp.StatusCode}. Ensure the registry is running and is accessible.");
                            }
                        }
                    }
                }
            }
        }
    }
}
