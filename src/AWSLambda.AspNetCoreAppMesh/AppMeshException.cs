using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh
{
    public class AppMeshException : Exception
    {
        public AppMeshException(string message) : base(message) { }

        public AppMeshException(string message, Exception innerException) : base(message, innerException) { }
    }
}
