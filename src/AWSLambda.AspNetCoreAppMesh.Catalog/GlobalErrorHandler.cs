using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;


namespace AWSLambda.AspNetCoreAppMesh.Catalog
{
    public class GlobalErrorHandler
    {
        public static async Task ExceptionHandlerDelegate(HttpContext context)
        {
            var resp = context.Response;

            resp.StatusCode = 500;

            var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();

            var logger = context.RequestServices.GetService<ILogger<GlobalErrorHandler>>();

            string errorMsg;

            if (exceptionHandler == null)
            {
                errorMsg = $"Unknown error occured. {nameof(IExceptionHandlerFeature)} is not available";

                logger.LogError(errorMsg);

                await resp.WriteAsync(errorMsg);

                return;
            }

            var ex = exceptionHandler.Error;

            if (ex == null)
            {
                errorMsg = "Unknown error occured";

                logger.LogError(errorMsg);

                await resp.WriteAsync(errorMsg);

                return;
            }

            logger.LogError(ex, "Error occured");

            await resp.WriteAsync($"Error: {ex.ToString()}");
        }
    }
}
