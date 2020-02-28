using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.TestUtilities;
using AWSLambda.AspNetCoreAppMesh.Config;
using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh
{
    public class HandleIncomingInvokeRequestsMiddleware
    {
        readonly RequestDelegate next;
        readonly ILogger<HandleIncomingInvokeRequestsMiddleware> logger;
        readonly ILambdaAppMeshOptions opts;
        readonly IServiceProvider services;

        public HandleIncomingInvokeRequestsMiddleware(RequestDelegate next, ILogger<HandleIncomingInvokeRequestsMiddleware> logger, IOptions<LambdaAppMeshOptions> opts, IServiceProvider services)
        {
            this.next = next;
            this.logger = logger;
            this.opts = opts.Value;
            this.services = services;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var req = context.Request;

            string lambdaName = req.Query["lambdaName"];
            string invocationType = req.Query["invocationType"];
            string payloadType = req.Query["payloadType"];
            string source = req.Query["source"];

            logger.LogInformation($"Received {payloadType} request from {source}");

            // todo -- handle dry run invocationType

            try
            {
                if (payloadType.Equals("APIGatewayProxyRequest", StringComparison.OrdinalIgnoreCase))
                {
                    await HandleAPIGatewayProxyRequest(context, lambdaName);
                }
                else if (payloadType.Equals("ApplicationLoadBalancerRequest", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotFiniteNumberException();
                }
                else
                {
                    throw new AppMeshException($"Unsupported payloadType type {payloadType} received from {source}");
                }
            }
            catch(Exception ex)
            {
                var errorMsg = $"Error processing {payloadType} from {source}";
                
                logger.LogError(ex, errorMsg);

                context.Response.StatusCode = 500;

                await context.Response.WriteAsync($"{errorMsg}: {ex.ToString()}");
            }
        }

        async Task HandleAPIGatewayProxyRequest(HttpContext context, string lambdaName)
        {
            var activator = (IAPIGatewayProxyFunctionActivator)services.GetService(typeof(IAPIGatewayProxyFunctionActivator));

            if (activator == null)
                throw new AppMeshException($"Ensure AddAPIGatewayProxyFunctionEntryPoint() has been called in ConfigureServices() method of your Startup.");

            var apiGatewayReq = JsonUtil.Deserialize<APIGatewayProxyRequest>(context.Request.Body);

            var func = activator.EntryPoint();

            var lambdaContext = new TestLambdaContext()
            {
                FunctionName = lambdaName
            };

            // todo: fill context

            var resp = await func.FunctionHandlerAsync(apiGatewayReq, lambdaContext);

            context.Response.StatusCode = resp.StatusCode;

            JsonUtil.SerializeAndLeaveOpen(context.Response.Body, resp);
        }
    }
}
