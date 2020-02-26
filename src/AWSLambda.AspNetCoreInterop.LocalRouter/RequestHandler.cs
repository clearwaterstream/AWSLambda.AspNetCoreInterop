using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public interface IRequestHandler
    {
        Task Home(HttpContext context);
        Task Clients(HttpContext context);
        Task Register(HttpContext context);
        Task ProxyRequest(HttpContext context);
    }
    
    public class RequestHandler : IRequestHandler
    {
        ILogger<RequestHandler> logger;

        public RequestHandler(ILogger<RequestHandler> logger)
        {
            this.logger = logger;
        }

        public Task Clients(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task Home(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task ProxyRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public Task Register(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
