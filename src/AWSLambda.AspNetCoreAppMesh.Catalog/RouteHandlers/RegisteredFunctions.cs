using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.Catalog.RouteHandlers
{
    public class RegisteredFunctions : IRouteHandler
    {
        readonly ILogger<RegisteredFunctions> logger;

        public RegisteredFunctions(ILogger<RegisteredFunctions> logger)
        {
            this.logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var sb = new StringBuilder();

            sb.AppendLine(Program.Title);
            sb.AppendLine();

            httpContext.Response.Headers["Content-Type"] = "text/plain";

            if(!Registrar.Instance.FunctionList.Any())
            {
                sb.Append("-- NO REGISTERED FUNCTIONS --");

                return httpContext.Response.WriteAsync(sb.ToString());
            }

            // todo -- redo the format below. User something like ConsoleTables

            sb.AppendLine($"Function Name\t\t\tUrl\t\t\t\tListening for Incoming Requests On");

            foreach(var f in Registrar.Instance.FunctionList)
            {
                var opts = f.Value;
                
                sb.AppendLine($"{opts.LambdaName}\t\t{opts.ApplicationUrl}\t\t{opts.HandlerPathForIncomingRequests}");
            }

            return httpContext.Response.WriteAsync(sb.ToString());
        }
    }
}
