using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AWSLambda.AspNetCoreAppMesh.Catalog
{
    using AWSLambda.AspNetCoreAppMesh.Util;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Server.Kestrel.Core;
    using Microsoft.Extensions.Hosting;
    using RouteHandlers;
    using System.Linq;

    public class Startup
    {           
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });

            services.AddLogging(opt => opt.AddConsole(c =>
            {
                c.TimestampFormat = "[HH:mm:ss.ffff] ";
            }));

            services.AddSingleton<Home>();
            services.AddSingleton<Register>();
            services.AddSingleton<GetFunctionInfo>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var logger = app.ApplicationServices.GetService<ILogger<Startup>>();
            
            var appLifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();

            if(appLifetime != null)
            {
                appLifetime.ApplicationStopping.Register(Program.OnShuttingDown);
            }
            
            app.UseExceptionHandler(new ExceptionHandlerOptions() { ExceptionHandler = GlobalErrorHandler.ExceptionHandlerDelegate });

            app.UseStaticFiles();

            app.UseRouting();

            var home = app.ApplicationServices.GetRequiredService<Home>();
            var register = app.ApplicationServices.GetRequiredService<Register>();
            var getFunctionInfo = app.ApplicationServices.GetRequiredService<GetFunctionInfo>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", home.Invoke);

                endpoints.MapGet("/function-info", getFunctionInfo.Invoke);

                endpoints.MapPost("/register", register.Invoke);
            });

            var addrF = app.ServerFeatures.Get<IServerAddressesFeature>();

            if (addrF != null && addrF.Addresses != null && addrF.Addresses.Any())
            {
                logger.LogInformation($"-- Use {addrF.Addresses.First()} to see a list of registered functions --");
            }
        }
    }
}
