using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AWSLambda.AspNetCoreAppMesh.Config;
using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AWSLambda.AspNetCoreAppMesh
{
    public interface ICatalogRegistrarAgent
    {
        Task RegisterFunction();
        Task<LambdaAppMeshOptions> GetFunctionInfo(string lambdaName);
    }

    public class CatalogRegistrarAgent : ICatalogRegistrarAgent
    {
        readonly HttpClient httpClient;
        readonly ILogger<CatalogRegistrarAgent> logger;
        readonly LambdaAppMeshOptions appMeshOptions;

        static readonly string ensureCatToolIsRunningMsg = "Ensure the catalog tool is running (lambda-mesh-cat) and that it is accessible. See https://github.com/clearwaterstream/aws-lambda-aspdotnetcore-appmesh for more info.";

        public CatalogRegistrarAgent(HttpClient httpClient, ILogger<CatalogRegistrarAgent> logger, IOptions<LambdaAppMeshOptions> opts)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            appMeshOptions = opts.Value;
        }

        public async Task RegisterFunction()
        {
            var url = UriUtil.Combine(appMeshOptions.CatalogUrl, "register");

            try
            {
                await RegisterFunctionInternal(url);
            }
            catch (AppMeshException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AppMeshException($"Error registering with catalog {appMeshOptions.CatalogUrl}. {ensureCatToolIsRunningMsg}", ex);
            }
        }

        public async Task<LambdaAppMeshOptions> GetFunctionInfo(string lambdaName)
        {
            var methodUrl = UriUtil.Combine(appMeshOptions.CatalogUrl, "function-info");

            var url = $"{methodUrl}?lambdaName={lambdaName}";

            try
            {
                using(var resp = await httpClient.GetAsync(url))
                {
                    if (resp.StatusCode == HttpStatusCode.NoContent)
                    {
                        throw new AppMeshException($"Function {lambdaName} is not registered with catalog {appMeshOptions.CatalogUrl}");
                    }
                    else if (!resp.IsSuccessStatusCode)
                    {
                        throw new AppMeshException($"Error obtaining function information from catalog {url}. Status code {resp.StatusCode}");
                    }

                    using(var stream = await resp.Content.ReadAsStreamAsync())
                    {
                        var opts = JsonUtil.Deserialize<LambdaAppMeshOptions>(stream);

                        return opts;
                    }
                }
            }
            catch(AppMeshException)
            {
                throw;
            }
            catch(Exception ex)
            {
                throw new AppMeshException($"Error obtaining function information from catalog {url}", ex);
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

                        JsonUtil.SerializeAndLeaveOpen(ms, appMeshOptions);

                        ms.Position = 0;

                        using (var resp = await httpClient.SendAsync(reqMsg))
                        {
                            if (!resp.IsSuccessStatusCode)
                            {
                                throw new AppMeshException($"Error registering with catalog {appMeshOptions.CatalogUrl}. Status code {resp.StatusCode}. {ensureCatToolIsRunningMsg}.");
                            }
                        }
                    }
                }
            }
        }
    }
}
