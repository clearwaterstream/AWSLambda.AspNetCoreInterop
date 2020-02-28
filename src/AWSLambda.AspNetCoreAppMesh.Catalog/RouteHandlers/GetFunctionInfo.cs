using System;
using System.Threading.Tasks;
using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AWSLambda.AspNetCoreAppMesh.Catalog.RouteHandlers
{
    public class GetFunctionInfo : IRouteHandler
    {
        readonly ILogger<GetFunctionInfo> logger;

        public GetFunctionInfo(ILogger<GetFunctionInfo> logger)
        {
            this.logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var functionName = httpContext.Request.Query["lambdaName"];

            var opts = Registrar.Instance.GetFunctionInfo(functionName);

            if(opts == null)
            {
                httpContext.Response.StatusCode = 204;

                logger.LogError($"Function {functionName} has not been registered.");

                return Task.CompletedTask;
            }

            httpContext.Response.StatusCode = 200;
            httpContext.Response.Headers["Content-Type"] = "application/json";

            JsonUtil.SerializeAndLeaveOpen(httpContext.Response.Body, opts);

            return Task.CompletedTask;
        }
    }
}
