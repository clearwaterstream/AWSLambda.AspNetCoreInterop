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

        static void Main(string[] args)
        {
            ConsoleUtil.WriteProgramTitle("AWS Lambda ASP .NET Core Interop");

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
