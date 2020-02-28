using AWSLambda.AspNetCoreInterop.Config;
using AWSLambda.AspNetCoreInterop.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.Registry.RouteHandlers
{
    public class Register : IRouteHandler
    {
        readonly ILogger<Register> logger;

        public Register(ILogger<Register> logger)
        {
            this.logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var opts = JsonUtil.Deserialize<LambdaInteropOptions>(request.Body);

            Registrar.Instance.RegisterFunction(opts);

            httpContext.Response.StatusCode = 200;
            
            await httpContext.Response.WriteAsync("ok");
        }
    }
}
