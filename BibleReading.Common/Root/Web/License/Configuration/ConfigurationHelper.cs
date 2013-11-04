using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Configuration;
using System.Web;
using System.Diagnostics;

namespace BibleReading.Common45.Root.Web.License.Configuration
{
    public static class ConfigurationHelper
    {
        public static LicenseGroup GetCurrentComponentConfiguration()
        {
            return GetCurrentConfiguration().SectionGroups["licenseGroup"] as LicenseGroup;
        }

        public static System.Configuration.Configuration GetCurrentConfiguration()
        {
            if (GetIsWeb())
            {
                return WebConfigurationManager.OpenWebConfiguration("~/");
            }
            else
            {
                return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
        }

        public static Boolean GetIsWeb()
        {
            if (HttpContext.Current == null)
            {
                // NON-WEB

                // Check for WCF hosted in IIS - if you have a better way, please email me!
                String processName = Process.GetCurrentProcess().ProcessName;
                if (processName.Equals("w3wp", StringComparison.InvariantCultureIgnoreCase) // IIS process
                    || processName.Equals("WebDev.WebServer", StringComparison.InvariantCultureIgnoreCase) // Cassini: .NET 2.0-3.5
                    || processName.Equals("WebDev.WebServer40", StringComparison.InvariantCultureIgnoreCase)) // Cassini: .NET 4.0
                {
                    // This is WCF running in IIS/Cassini
                    return true;
                }
                else
                {
                    // Regular non-web app: winforms, console, windows service, etc
                    return false;
                }

            }
            else
            {
                // WEB
                return true;
            }

        }
    }
}
