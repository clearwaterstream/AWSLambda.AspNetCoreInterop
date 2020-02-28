using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public interface IApplicationUrlResolver
    {
        string GetApplicationUrl();
    }
}
