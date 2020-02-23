using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
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
            services.AddLogging(opt => opt.AddConsole(c =>
            {
                c.TimestampFormat = "[HH:mm:ss.ffff] ";
            }));

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var appLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

            appLifetime.ApplicationStopping.Register(Program.OnShuttingDown);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", HomePage.Render);

                endpoints.MapGet("/clients", ClientsPage.Render);

                endpoints.MapHub<PigeonPost>("/pigeon-post");
            });
        }
    }
}
