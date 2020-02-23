using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public static class ClientsPage
    {
        public static Task Render(HttpContext context)
        {
            return context.Response.WriteAsync("todo");
        }
    }
}
