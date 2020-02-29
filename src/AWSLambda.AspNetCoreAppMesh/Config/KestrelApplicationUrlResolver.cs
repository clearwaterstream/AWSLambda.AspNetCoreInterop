using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public class KestrelApplicationUrlResolver : IApplicationUrlResolver
    {
        readonly IServer server;
        
        public KestrelApplicationUrlResolver(IServer server)
        {
            this.server = server;
        }
        
        public string GetApplicationUrl()
        {
            var f = server.Features.Get<IServerAddressesFeature>();

            if (f == null || f.Addresses == null || !f.Addresses.Any())
                return null;

            var addr = f.Addresses.First();

            return addr;
        }
    }
}
