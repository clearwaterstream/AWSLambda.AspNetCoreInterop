using AWSLambda.AspNetCoreInterop.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreInterop.Registry
{
    public class Registrar
    {
        private Registrar() { }

        public static Registrar Instance { get; } = new Registrar();

        readonly Dictionary<string, LambdaInteropOptions> _list = new Dictionary<string, LambdaInteropOptions>();

        public void RegisterFunction(LambdaInteropOptions options)
        {
            var k = options.LambdaName.ToLowerInvariant();

            _list[k] = options;
        }

        public LambdaInteropOptions GetFunctionInfo(string lambdaName)
        {
            var k = lambdaName.ToLowerInvariant();

            if (_list.TryGetValue(k, out LambdaInteropOptions opts))
                return opts;

            return null;
        }
    }
}
