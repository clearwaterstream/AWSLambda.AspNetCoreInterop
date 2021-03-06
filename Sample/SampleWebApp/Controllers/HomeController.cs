﻿using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Model;
using AWSLambda.AspNetCoreAppMesh;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SampleWebApp.Controllers
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public IActionResult Hello()
        {
            return Content("Hello, MyAspNetCoreLambda at your service.");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SayHelloToSelf()
        {
            // call Hello() via routing the InvokeRequest locally (over http)

            var invokeReq = new InvokeRequest();
            invokeReq.FunctionName = "MyAspNetCoreLambda"; // call self
            invokeReq.InvocationType = InvocationType.RequestResponse;

            var apiGatewayReq = new APIGatewayProxyRequest()
            {
                HttpMethod = "GET",
                Path = "/home/hello"
            };

            using var payloadStream = new MemoryStream();

            await JsonSerializer.SerializeAsync(payloadStream, apiGatewayReq);
            
            payloadStream.Position = 0;

            invokeReq.PayloadStream = payloadStream;

            var resp = await invokeReq.RouteAPIGatewayProxyRequestLocally();

            return Content(resp.Body);
        }
    }
}
