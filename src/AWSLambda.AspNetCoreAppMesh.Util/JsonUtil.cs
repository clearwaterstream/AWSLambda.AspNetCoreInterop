using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AWSLambda.AspNetCoreAppMesh.Util
{
    public static class JsonUtil
    {
        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T));
        }

        // cpu intensive, do NOT needlessly async this!
        public static object Deserialize(Stream stream, Type objectType)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var sr = new StreamReader(stream))
            {
                using (var tr = new JsonTextReader(sr))
                {
                    var ser = JsonSerializer.Create();

                    return ser.Deserialize(tr, objectType);
                }
            }
        }

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static void Serialize(Stream stream, object value)
        {
            if (value == null)
                return; // if the value is null, nothing to do here

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var sw = new StreamWriter(stream))
            {
                using (var tw = new JsonTextWriter(sw))
                {
                    var ser = JsonSerializer.Create();

                    ser.Serialize(tw, value);
                }
            }
        }

        public static void SerializeAndLeaveOpen(Stream stream, object value)
        {
            if (value == null)
                return; // if the value is null, nothing to do here

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            // in .NET Core 3 you can just do new StreamWriter(stream, leaveOpen: true)
            using (var sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
            {
                using (var tw = new JsonTextWriter(sw))
                {
                    var ser = JsonSerializer.Create();

                    ser.Serialize(tw, value);
                }
            }
        }
    }
}
