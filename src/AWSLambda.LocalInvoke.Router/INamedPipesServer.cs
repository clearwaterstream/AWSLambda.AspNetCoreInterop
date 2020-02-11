using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AWSLambda.LocalInvoke.Router
{
    public interface INamedPipesServer : IDisposable
    {
        string PipeName { get; set; }
        void Start(CancellationToken cancellationToken);
    }
}
