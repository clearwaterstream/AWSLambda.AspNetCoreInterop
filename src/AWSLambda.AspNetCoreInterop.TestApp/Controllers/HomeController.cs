using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.TestApp.Controllers
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public IActionResult Hello()
        {
            return Content("hello yourself");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult MirrorHello()
        {
            
            
            return Content("ok");
        }
    }
}
