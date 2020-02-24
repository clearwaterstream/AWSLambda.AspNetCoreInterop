using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Client
{
    public class InteropClientException : Exception
    {
        public InteropClientException(string message) : base(message) { }

        public InteropClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
