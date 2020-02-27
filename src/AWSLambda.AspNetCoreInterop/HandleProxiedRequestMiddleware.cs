using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.AspNetCoreServer;
using Amazon.Lambda.TestUtilities;
using AWSLambda.AspNetCoreInterop.Config;
using AWSLambda.AspNetCoreInterop.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop
{
    public class HandleProxiedRequestMiddleware
    {
        readonly RequestDelegate next;
        readonly ILogger<HandleProxiedRequestMiddleware> logger;
        readonly ILambdaInteropOptions opts;
        readonly IServiceProvider services;

        public HandleProxiedRequestMiddleware(RequestDelegate next, ILogger<HandleProxiedRequestMiddleware> logger, IOptions<LambdaInteropOptions> opts, IServiceProvider services)
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
            string payloadType = req.Query["payloadType"];
            string source = req.Query["source"];

            logger.LogInformation($"Received {payloadType} request from {source}");

            try
            {
                if (payloadType.Equals("APIGatewayProxyRequest", StringComparison.OrdinalIgnoreCase))
                {
                    await HandleAPIGatewayProxyRequest(context);
                }
                else if (payloadType.Equals("ApplicationLoadBalancerRequest", StringComparison.OrdinalIgnoreCase))
                {
                    throw new NotFiniteNumberException();
                }
                else
                {
                    throw new InteropException($"Unsupported payloadType type {payloadType} received from {source}");
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

        async Task HandleAPIGatewayProxyRequest(HttpContext context)
        {
            var activator = (IAPIGatewayProxyFunctionActivator)services.GetService(typeof(IAPIGatewayProxyFunctionActivator));

            if (activator == null)
                throw new InteropException($"Ensure AddAPIGatewayProxyFunctionEntryPoint() has been called in ConfigureServices() method of your Startup.");

            var apiGatewayReq = JsonUtil.Deserialize<APIGatewayProxyRequest>(context.Request.Body);

            var func = activator.EntryPoint();

            var lambdaContext = new TestLambdaContext();

            // todo: fill context

            var resp = await func.FunctionHandlerAsync(apiGatewayReq, lambdaContext);

            context.Response.StatusCode = resp.StatusCode;

            AWSUtils.SetHeadersCollection(context.Response.Headers, resp.Headers, resp.MultiValueHeaders);

            JsonUtil.SerializeAndLeaveOpen(context.Response.Body, resp);
        }
    }
}
