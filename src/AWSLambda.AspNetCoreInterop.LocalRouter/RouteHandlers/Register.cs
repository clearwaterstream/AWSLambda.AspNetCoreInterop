using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter.RouteHandlers
{
    public class Register : IRouteHandler
    {
        readonly Logger<Register> logger;

        public Register(Logger<Register> logger)
        {
            this.logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            throw new NotImplementedException();
        }
    }
}
