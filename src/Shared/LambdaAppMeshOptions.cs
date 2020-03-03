﻿using AWSLambda.AspNetCoreAppMesh.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public interface ILambdaAppMeshOptions
    {
        string LambdaName { get; set; }
        string CatalogUrl { get; set; }
        string ApplicationUrl { get; set; }
        string HandlerPathForIncomingRequests { get; set; }
        bool HandleIncomingRequestsInDevelopmentOnly { get; set; }
    }

    public class LambdaAppMeshOptions : ILambdaAppMeshOptions
    {
        public string LambdaName { get; set; }
        public string CatalogUrl { get; set; }
        public string ApplicationUrl { get; set; }
        public string HandlerPathForIncomingRequests { get; set; } = "/lambda-invoke-handler";
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
