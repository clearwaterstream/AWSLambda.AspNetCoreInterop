using AWSLambda.AspNetCoreInterop.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop
{
    public class ProxiedRequestHandlerMiddleware
    {
        public ProxiedRequestHandlerMiddleware(IOptions<LambdaInteropOptions> config, ILogger<ProxiedRequestHandlerMiddleware> logger, IRouterClientService routerHttpClient)
        {

        }
    }
}
