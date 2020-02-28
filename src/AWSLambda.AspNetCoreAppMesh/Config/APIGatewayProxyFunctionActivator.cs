using Amazon.Lambda.AspNetCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public interface IAPIGatewayProxyFunctionActivator
    {
        APIGatewayProxyFunction EntryPoint();
    }

    public class APIGatewayProxyFunctionActivator<TEntryPoint> : IAPIGatewayProxyFunctionActivator where TEntryPoint : APIGatewayProxyFunction, new()
    {
        readonly TEntryPoint entryPoint = new TEntryPoint();
        
        public APIGatewayProxyFunction EntryPoint()
        {
            return entryPoint;
        }
    }
}
