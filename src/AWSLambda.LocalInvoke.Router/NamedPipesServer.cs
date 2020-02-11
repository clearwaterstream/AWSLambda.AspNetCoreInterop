using Amazon.Lambda.Model;
using AWSLambda.LocalInvoke.Common;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.LocalInvoke.Router
{
    public class NamedPipesServer : INamedPipesServer
    {
        readonly ILogger logger;

        public string PipeName { get; set; } = "AWSLambda.LocalInvoke.Router";

        public NamedPipesServer(ILogger<NamedPipesServer> logger)
        {
            this.logger = logger;
        }

        public void Start(CancellationToken cancellationToken)
        {
            var maxNumOfServers = System.Environment.ProcessorCount;

            var tasks = new Task[maxNumOfServers];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    await RunServer(maxNumOfServers, cancellationToken);
                });
            }

            Task.WhenAll(tasks);
        }

        async Task RunServer(int maxNumOfServers, CancellationToken cancellationToken)
        {
            var threadId = Thread.CurrentThread.ManagedThreadId;

            using var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, maxNumOfServers);

            if (cancellationToken.IsCancellationRequested)
                return;

            logger.LogInformation($"server listening on thread {threadId}");

            do
            {
                await pipeServer.WaitForConnectionAsync(cancellationToken);

                logger.LogInformation($"client connected on thread {threadId}");

                var invokeRequest = JsonUtil.Deserialize<InvokeRequest>(pipeServer);

            }
            while (!cancellationToken.IsCancellationRequested);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
