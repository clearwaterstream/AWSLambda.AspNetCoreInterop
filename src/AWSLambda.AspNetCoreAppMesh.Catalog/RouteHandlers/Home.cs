using AWSLambda.AspNetCoreAppMesh.Catalog.Util;
using AWSLambda.AspNetCoreAppMesh.Config;
using AWSLambda.AspNetCoreAppMesh.Util;
using ConsoleTables;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            httpContext.Response.Headers["Content-Type"] = "text/plain";
            httpContext.Response.StatusCode = 200;

            var sb = new StringBuilder();

            ConsoleUtil.WriteProgramTitle(Program.Title, sb);
            sb.AppendLine();

            if (!Registrar.Instance.FunctionList.Any())
            {
                sb.AppendLine("-- NO REGISTERED FUNCTIONS --");

                return httpContext.Response.WriteAsync(sb.ToString());
            }

            var table = new ConsoleTable("Function Name", "Listening On");

            foreach (var kvp in Registrar.Instance.FunctionList)
            {
                var opts = kvp.Value;

                table.AddRow(opts.LambdaName, opts.ListensOn());
            }

            sb.Append(table.ToString());

            return httpContext.Response.WriteAsync(sb.ToString());
        }
    }
}
