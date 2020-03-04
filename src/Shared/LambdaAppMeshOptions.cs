using AWSLambda.AspNetCoreAppMesh.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public class LambdaAppMeshOptions
    {
        /// <summary>
        /// Name of this Lambda Function
        /// </summary>
        public string LambdaName { get; set; }
        /// <summary>
        /// Url of the Catalog Tool (dotnet lambda-app-mesh)
        /// </summary>
        public string CatalogUrl { get; set; }
        /// <summary>
        /// Url of this application when running locally (the library will try to infer from launchSettings.json)
        /// </summary>
        public string ApplicationUrl { get; set; }
        /// <summary>
        /// Path to use to handle incoming Invoke requests, defaults to /lambda-invoke-handler
        /// </summary>
        public string HandlerPathForIncomingRequests { get; set; } = "/lambda-invoke-handler";
        /// <summary>
        /// Listen for incoming requests in Development environment only (safe-guard). Default is true
        /// </summary>
        public bool HandleIncomingRequestsInDevelopmentOnly { get; set; } = true;
    }

    public static class LambdaAppMeshOptionsExtensions
    {
        public static string ListensOn(this LambdaAppMeshOptions opts)
        {
            if (opts == null)
                return null;

            var result = UriUtil.Combine(opts.ApplicationUrl, opts.HandlerPathForIncomingRequests);

            return result;
        }
    }
}
