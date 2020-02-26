using System;
using System.Collections.Generic;
using System.Text;

namespace AWSLambda.AspNetCoreInterop
{
    public class InteropException : Exception
    {
        public InteropException(string message) : base(message) { }

        public InteropException(string message, Exception innerException) : base(message, innerException) { }
    }
}
