using AWSLambda.AspNetCoreAppMesh.Catalog.Config;
using AWSLambda.AspNetCoreAppMesh.Catalog.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.Catalog
{
    public class Program
    {
        static ILogger<Program> logger;

        public static readonly string Title = "AWS Lambda ASP.NET Core App Mesh - Catalog";

        static AppSettings settings;

        static async Task Main(string[] args)
        {
            ConsoleUtil.WriteProgramTitle(Title);

            settings = await AppSettingsResolver.Load(args);

            if(settings != null)
            {
                args = settings.Args;
            }

            try
            {
                Run(args);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Startup error: {ex.ToString()}");
            }
        }

        static void Run(string[] args)
        {
            var builder = CreateHostBuilder(args);

            using var host = builder.Build();
            
            logger = host.Services.GetService<ILogger<Program>>();

            logger.LogInformation("Catalog bootstrapped");

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static async Task OnShuttingDown()
        {
            if(settings != null)
            {
                await AppSettingsResolver.Save(settings);
            }
        }
    }
}
