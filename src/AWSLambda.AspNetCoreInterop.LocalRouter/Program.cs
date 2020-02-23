using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    class Program
    {
        static ILogger<Program> logger;
        static readonly CancellationTokenSource appCtSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            ConsoleUtil.WriteProgramTitle("AWS Lambda ASP .NET Core Interop");

            var builder = CreateHostBuilder(args);

            var host = builder.Build();

            logger = host.Services.GetService<ILogger<Program>>();

            logger.LogInformation("Router bootstrapped");

            host.Run();

            Console.Read();
        }

        public static void OnShuttingDown()
        {            
            appCtSource.Cancel();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
