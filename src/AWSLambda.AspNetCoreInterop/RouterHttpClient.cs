using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace AWSLambda.AspNetCoreInterop
{
    public interface IRouterHttpClient
    {
    }

    public class RouterHttpClient : IRouterHttpClient
    {
        readonly HttpClient httpClient;
        readonly ILogger<RouterHttpClient> logger;

        public RouterHttpClient(HttpClient httpClient, ILogger<RouterHttpClient> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }
    }
}
