// Baseado em http://robseder.wordpress.com/articles/the-complete-guide-to-custom-configuration-sections/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BibleReading.Common45.Root.Web.License.Configuration
{
    public class LicenseSection : ConfigurationSection
    {
        // Create a "ServerName" attribute.
        [ConfigurationProperty("serverName", IsRequired = true)]
        public string ServerName
        {
            get
            {
                return this["serverName"].ToString();
            }
            set
            {
                this["serverName"] = value;
            }
        }
    }
}
