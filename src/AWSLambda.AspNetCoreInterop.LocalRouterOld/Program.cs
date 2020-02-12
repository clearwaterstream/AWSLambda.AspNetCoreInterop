using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    class Program
    {
        static ServiceProvider serviceProvider;
        static ILogger<Program> logger;
        static readonly CancellationTokenSource appCtSource = new CancellationTokenSource();
        
        static void Main(string[] args)
        {
            ConsoleUtil.WriteProgramTitle("AWS Lambda ASP .NET Core Interop");
            
            AppDomain.CurrentDomain.ProcessExit += AppDomain_ProcessExit;

            Bootstrap();

            logger = serviceProvider.GetService<ILogger<Program>>();

            logger.LogInformation("Router bootstrapped");

            var server = serviceProvider.GetService<INamedPipesServer>();

            server.Start(appCtSource.Token);

            Console.Read();
        }

        static void Bootstrap()
        {
            var services = new ServiceCollection();

            services.AddLogging(opt => opt.AddConsole(c =>
            {
                c.TimestampFormat = "[HH:mm:ss.ffff] ";
            }));

            services.AddSingleton<INamedPipesServer, NamedPipesServer>();

            serviceProvider = services.BuildServiceProvider();
        }

        static void AppDomain_ProcessExit(object sender, EventArgs e)
        {
            appCtSource.Cancel();
        }
    }
}
