using AWSLambda.AspNetCoreInterop.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public class ProxiedRequestHandlerMiddleware
    {
        readonly RequestDelegate next;
        readonly ILogger<ProxiedRequestHandlerMiddleware> logger;
        readonly IRouterClientService routerClientService;

        public ProxiedRequestHandlerMiddleware(RequestDelegate next, ILogger<ProxiedRequestHandlerMiddleware> logger, IRouterClientService routerClientService)
        {
            this.next = next;
            this.logger = logger;
            this.routerClientService = routerClientService;
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Response.StatusCode = 200;
            
            return context.Response.WriteAsync("todo");
        }
    }
}
