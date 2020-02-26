using AWSLambda.AspNetCoreInterop.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop
{
    public class ProxiedLambdaRequestHandlerMiddleware
    {
        public ProxiedLambdaRequestHandlerMiddleware(IOptions<LambdaInteropOptions> config, ILogger<ProxiedLambdaRequestHandlerMiddleware> logger, IRouterHttpClient routerHttpClient)
        {

        }
    }
}
