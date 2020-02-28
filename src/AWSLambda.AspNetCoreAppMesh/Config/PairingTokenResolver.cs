using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AWSLambda.AspNetCoreAppMesh.Config
{
    public interface IPairingTokenResolver
    {
        (string headerName, string token) GetToken();
    }

    public class PairingTokenResolver : IPairingTokenResolver
    {
        static readonly string HEADER_NAME = "MS-ASPNETCORE-TOKEN";
        readonly IStartupFilter iisSetupFilter;
        readonly FieldInfo pairingTokenField;
        
        public PairingTokenResolver(IServiceProvider services)
        {
            var startupFilters = services.GetServices<IStartupFilter>();

            if (startupFilters != null)
            {
                iisSetupFilter = startupFilters.FirstOrDefault(x => x.GetType().Name == "IISSetupFilter");

                if (iisSetupFilter != null)
                {
                    pairingTokenField = iisSetupFilter.GetType().GetField("_pairingToken", BindingFlags.Instance | BindingFlags.NonPublic);
                }
            }
        }

        public (string headerName, string token) GetToken()
        {
            if(pairingTokenField != null)
            {
                var pairingToken = (string)pairingTokenField.GetValue(iisSetupFilter);

                return (HEADER_NAME, pairingToken);
            }
            else
            {
                return (null, null);
            }
        }
    }
}
