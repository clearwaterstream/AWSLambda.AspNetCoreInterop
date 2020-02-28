// https://github.com/aws/aws-lambda-dotnet/blob/master/Libraries/src/Amazon.Lambda.AspNetCoreServer/Internal/Utilities.cs

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Utils
{
    public static class AWSUtils
    {
        public static void SetHeadersCollection(HttpHeaders headers, IDictionary<string, string> singleValues, IDictionary<string, IList<string>> multiValues)
        {
            if (multiValues?.Count > 0)
            {
                foreach (var kvp in multiValues)
                {
                    headers.Add(kvp.Key, kvp.Value);
                }
            }
            else if (singleValues?.Count > 0)
            {
                foreach (var kvp in singleValues)
                {
                    headers.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public static MemoryStream ConvertLambdaRequestBodyToAspNetCoreBody(string body, bool isBase64Encoded)
        {
            Byte[] binaryBody;
            if (isBase64Encoded)
            {
                binaryBody = Convert.FromBase64String(body);
            }
            else
            {
                binaryBody = UTF8Encoding.UTF8.GetBytes(body);
            }

            return new MemoryStream(binaryBody);
        }
    }
}