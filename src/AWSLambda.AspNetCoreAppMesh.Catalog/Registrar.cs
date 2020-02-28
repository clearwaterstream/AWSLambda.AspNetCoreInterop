using AWSLambda.AspNetCoreAppMesh.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AWSLambda.AspNetCoreAppMesh.Catalog
{
    public class Registrar
    {
        private Registrar() { }

        public static Registrar Instance { get; } = new Registrar();

        readonly Dictionary<string, LambdaAppMeshOptions> _list = new Dictionary<string, LambdaAppMeshOptions>();

        public void RegisterFunction(LambdaAppMeshOptions options)
        {
            var k = options.LambdaName.ToLowerInvariant();

            _list[k] = options;
        }

        public LambdaAppMeshOptions GetFunctionInfo(string lambdaName)
        {
            var k = lambdaName.ToLowerInvariant();

            if (_list.TryGetValue(k, out LambdaAppMeshOptions opts))
                return opts;

            return null;
        }
    }
}
