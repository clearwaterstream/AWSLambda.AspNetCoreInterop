using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.Registry.RouteHandlers
{
    public class Home : IRouteHandler
    {
        readonly ILogger<Home> logger;

        public Home(ILogger<Home> logger)
        {
            this.logger = logger;
        }
        
        public Task Invoke(HttpContext httpContext)
        {
            return httpContext.Response.WriteAsync("AWS Lambda ASP .NET Core Interop - Function Registry");
        }
    }
}
