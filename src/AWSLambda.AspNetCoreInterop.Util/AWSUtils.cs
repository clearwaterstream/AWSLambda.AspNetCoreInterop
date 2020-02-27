// https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.AspNetCoreServer/Internal/Utilities.cs

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Util
{
    public static class AWSUtils
    {
        public static void SetHeadersCollection(IHeaderDictionary headers, IDictionary<string, string> singleValues, IDictionary<string, IList<string>> multiValues)
        {
            if (multiValues?.Count > 0)
            {
                foreach (var kvp in multiValues)
                {
                    headers[kvp.Key] = new StringValues(kvp.Value.ToArray());
                }
            }
            else if (singleValues?.Count > 0)
            {
                foreach (var kvp in singleValues)
                {
                    headers[kvp.Key] = new StringValues(kvp.Value);
                }
            }
        }
    }
}
