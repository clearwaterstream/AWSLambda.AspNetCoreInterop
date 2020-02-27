using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AWSLambda.AspNetCoreInterop.Config
{
    public class FromLaunchSettingsApplicationUrlResolver : IApplicationUrlResolver
    {
        readonly ILogger<FromLaunchSettingsApplicationUrlResolver> logger;

        public FromLaunchSettingsApplicationUrlResolver(ILogger<FromLaunchSettingsApplicationUrlResolver> logger)
        {
            this.logger = logger;
        }

        public string GetApplicationUrl()
        {
            var path = Path.Combine("Properties", "launchSettings.json");

            try
            {
                var appUrl = GetApplicationUrl(path);

                return appUrl;
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Error resolving ApplicationUrl from {path}");
            }

            return null;
        }

        string GetApplicationUrl(string path)
        {
            if (!File.Exists(path))
                return null;

            JObject jObject = null;

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))
                {
                    using (var jr = new JsonTextReader(sr))
                    {
                        jObject = JObject.Load(jr);
                    }
                }
            }

            var token = jObject.SelectToken("iisSettings.iisExpress.applicationUrl");

            if (token == null)
                return null;

            var appUrl = token.Value<string>();

            logger.LogInformation($"Resolved applicationUrl as {appUrl} from {path}");

            return appUrl;
        }
    }
}
