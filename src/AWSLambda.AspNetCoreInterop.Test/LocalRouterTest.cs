using System;
using Amazon.Lambda.AspNetCoreServer;
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
        public void ListenForProxiedRequests()
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
            services.AddAWSLambdaInteropClient(opts =>
            {
                opts.LambdaName = "test";
                opts.RouterUrl = "http://localhost:5050";
            });
        }

        public virtual void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAWSLambdaInteropClient();
            app.HandleIncomingAPIGatewayProxyRequests<ApiGatewayEntryPoint>(env);
        }
    }

    class ApiGatewayEntryPoint : APIGatewayProxyFunction
    {
        protected override void Init(IWebHostBuilder builder)
        {
            builder.UseStartup<TestStartup>();
        }
    }
}
