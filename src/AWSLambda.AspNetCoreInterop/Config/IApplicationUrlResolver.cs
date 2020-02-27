using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Config
{
    public interface IApplicationUrlResolver
    {
        string GetApplicationUrl();
    }
}
