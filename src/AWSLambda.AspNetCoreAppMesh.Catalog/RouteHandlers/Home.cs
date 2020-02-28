using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.Catalog.RouteHandlers
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
            return httpContext.Response.WriteAsync(Program.Title);
        }
    }
}
