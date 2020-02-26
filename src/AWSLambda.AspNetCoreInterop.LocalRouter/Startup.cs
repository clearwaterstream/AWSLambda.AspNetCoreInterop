using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    using RouteHandlers;
    
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(opt => opt.AddConsole(c =>
            {
                c.TimestampFormat = "[HH:mm:ss.ffff] ";
            }));

            services.AddSingleton<Home>();
            services.AddSingleton<Register>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            var home = app.ApplicationServices.GetRequiredService<Home>();
            var register = app.ApplicationServices.GetRequiredService<Register>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", home.Invoke);

                //endpoints.MapGet("/clients", reqHandler.Clients);

                endpoints.MapPost("/register", register.Invoke);

                //endpoints.MapPost("/proxy-request", reqHandler.ProxyRequest);
            });
        }
    }
}
