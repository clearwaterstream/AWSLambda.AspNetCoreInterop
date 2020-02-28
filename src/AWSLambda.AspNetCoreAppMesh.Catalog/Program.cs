using AWSLambda.AspNetCoreAppMesh.Catalog.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace AWSLambda.AspNetCoreAppMesh.Catalog
{
    class Program
    {
        static ILogger<Program> logger;

        public static readonly string Title = "AWS Lambda ASP .NET Core App Mesh - Catalog";

        static void Main(string[] args)
        {
            ConsoleUtil.WriteProgramTitle(Title);

            try
            {
                Run(args);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine($"Startup error: {ex.ToString()}");
            }

            Console.Read();
        }

        static void Run(string[] args)
        {
            var builder = CreateHostBuilder(args);

            using (var host = builder.Build())
            {
                logger = host.Services.GetService<ILogger<Program>>();

                logger.LogInformation("Router bootstrapped");

                host.Run();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
