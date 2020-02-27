using AWSLambda.AspNetCoreInterop.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.LocalRouter
{
    public class ClientList
    {
        private ClientList() { }

        public static ClientList Instance { get; } = new ClientList();

        readonly Dictionary<string, LambdaInteropOptions> _clients = new Dictionary<string, LambdaInteropOptions>();

        public void AddClient(LambdaInteropOptions options)
        {
            var k = options.LambdaName.ToLowerInvariant();

            _clients[k] = options;
        }

        public LambdaInteropOptions GetClientInfo(string lambdaName)
        {
            var k = lambdaName.ToLowerInvariant();

            if (_clients.TryGetValue(k, out LambdaInteropOptions opts))
                return opts;

            return null;
        }
    }
}
