using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Util
{
    public static class JsonUtil
    {
        public static readonly JsonSerializerSettings LeanSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static readonly JsonSerializerSettings LeanSerializerSettings_withTypeInfo = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            Formatting = Formatting.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.All
        };

        public static readonly JsonSerializerSettings NiceSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Include,
            DefaultValueHandling = DefaultValueHandling.Include,
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public static T Deserialize<T>(Stream stream)
        {
            return (T)Deserialize(stream, typeof(T), null);
        }

        public static T Deserialize<T>(Stream stream, JsonSerializerSettings settings)
        {
            return (T)Deserialize(stream, typeof(T), settings);
        }

        public static object Deserialize(Stream stream, Type objectType)
        {
            return Deserialize(stream, objectType, null);
        }

        // cpu intensive, do NOT needlessly async this!
        public static object Deserialize(Stream stream, Type objectType, JsonSerializerSettings settings)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var sr = new StreamReader(stream))
            {
                using (var tr = new JsonTextReader(sr))
                {
                    var ser = JsonSerializer.Create(settings);

                    return ser.Deserialize(tr, objectType);
                }
            }
        }

        public static string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, LeanSerializerSettings);
        }

        public static string Serialize(object value, JsonSerializerSettings settings)
        {
            return JsonConvert.SerializeObject(value, settings);
        }

        public static void Serialize(Stream stream, object value, JsonSerializerSettings settings)
        {
            if (value == null)
                return; // if the value is null, nothing to do here

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var sw = new StreamWriter(stream))
            {
                using (var tw = new JsonTextWriter(sw))
                {
                    var ser = JsonSerializer.Create(settings);

                    ser.Serialize(tw, value);
                }
            }
        }
    }
}
