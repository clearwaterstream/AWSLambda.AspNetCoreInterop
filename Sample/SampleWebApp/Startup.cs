using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddRazorPages();

            services.AddAWSLambdaAppMeshClient(opts =>
            {
                opts.LambdaName = "MyAspNetCoreLambda"; // name of your Lambda function
                opts.CatalogUrl = "http://localhost:5050"; // URL the catalog tool (dotnet lambda-app-mesh) is listening on
            });
            
            services.AddAPIGatewayProxyFunctionEntryPoint<LambdaEntryPoint>(); // your APIGatewayProxyFunction entry point
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseAWSLambdaAppMeshClient();
            app.HandleIncomingAWSLambdaInvokeRequests(env);

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
