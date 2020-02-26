using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AWSLambda.AspNetCoreInterop.Test
{
    public class LocalRouterTest
    {
    }

    class TestStartup
    {
        public TestStartup(IConfiguration configuration, IHostingEnvironment env)
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {

        }
    }
}
