using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AWSLambda.AspNetCoreInterop.Test
{
    public class LocalRouterTest
    {
        [Fact]
        public void RegisterWithRouter()
        {
            var builder = WebHost.CreateDefaultBuilder().UseStartup<TestStartup>();

            using (var server = new TestServer(builder))
            {

            }
        }
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
