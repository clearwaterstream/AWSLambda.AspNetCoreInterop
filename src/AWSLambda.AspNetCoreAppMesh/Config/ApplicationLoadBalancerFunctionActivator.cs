using Amazon.Lambda.AspNetCoreServer;
using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public interface IApplicationLoadBalancerFunctionActivator
    {
        ApplicationLoadBalancerFunction EntryPoint();
    }

    public class ApplicationLoadBalancerFunctionActivator<TEntryPoint> : IApplicationLoadBalancerFunctionActivator where TEntryPoint : ApplicationLoadBalancerFunction, new()
    {
        readonly TEntryPoint entryPoint = new TEntryPoint();
        
        public ApplicationLoadBalancerFunction EntryPoint()
        {
            return entryPoint;
        }
    }
}
