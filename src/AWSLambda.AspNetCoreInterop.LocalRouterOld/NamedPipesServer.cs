using Amazon.Lambda.Model;
using AWSLambda.AspNetCoreInterop.Util;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public class NamedPipesServer : INamedPipesServer
    {
        readonly ILogger logger;

        public string RouterName { get; set; } = "AWSLambda.LocalInvoke.Router";

        public NamedPipesServer(ILogger<NamedPipesServer> logger)
        {
            this.logger = logger;
        }

        public Task Start(CancellationToken cancellationToken)
        {
            var maxNumOfServers = System.Environment.ProcessorCount;

            logger.LogInformation($"Router name is {RouterName}");

            var tasks = new Task[maxNumOfServers];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    await RunServer(maxNumOfServers, cancellationToken);
                });
            }

            var allT = Task.WhenAll(tasks);

            return allT;
        }

        async Task RunServer(int maxNumOfServers, CancellationToken cancellationToken)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            using var pipeServer = new NamedPipeServerStream(RouterName, PipeDirection.InOut, maxNumOfServers);

            if (cancellationToken.IsCancellationRequested)
                return;

            logger.LogInformation($"server listening on thread {threadId}");

            do
            {
                await pipeServer.WaitForConnectionAsync(cancellationToken);

                logger.LogInformation($"client connected on thread {threadId}");

                var msg = ReadMessage(pipeServer);

                logger.LogInformation($"msg received on thread {threadId}: {msg}");

            }
            while (!cancellationToken.IsCancellationRequested);
        }

        string ReadMessage(Stream server)
        {
            using(var sr = new StreamReader(server, Encoding.UTF8, true, 1024, true))
            {
                var msg = sr.ReadToEnd();

                return msg;
            }
        }
    }
}
