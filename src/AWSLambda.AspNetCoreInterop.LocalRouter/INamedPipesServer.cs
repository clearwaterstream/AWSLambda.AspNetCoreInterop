using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public interface INamedPipesServer
    {
        string RouterName { get; set; }
        Task Start(CancellationToken cancellationToken);
    }
}
