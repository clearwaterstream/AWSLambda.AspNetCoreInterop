using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public interface IRouteHandler
    {
        public Task Invoke(HttpContext httpContext);
    }
}
