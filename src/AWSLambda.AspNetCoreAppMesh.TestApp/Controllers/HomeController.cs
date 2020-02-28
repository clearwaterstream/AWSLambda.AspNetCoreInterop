using Amazon.Lambda;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Model;
using AWSLambda.AspNetCoreAppMesh.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.TestApp.Controllers
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public IActionResult Hello()
        {
            return Content("hello yourself");
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> MirrorHello()
        {
            // call Hello() via routing the InvokeRequest locally (over http)

            var invokeReq = new InvokeRequest();
            invokeReq.FunctionName = "test";
            invokeReq.InvocationType = InvocationType.RequestResponse;

            var apiGatewayReq = new APIGatewayProxyRequest()
            {
                HttpMethod = "GET",
                Path = "/home/hello"
            };

            using (var payloadStream = new MemoryStream())
            {
                JsonUtil.SerializeAndLeaveOpen(payloadStream, apiGatewayReq);
                payloadStream.Position = 0;

                invokeReq.PayloadStream = payloadStream;

                var resp = await invokeReq.RouteAPIGatewayProxyRequestLocally();

                return Content(resp.Body);
            }
        }
    }
}
