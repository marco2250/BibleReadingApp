// Baseado em http://robseder.wordpress.com/articles/the-complete-guide-to-custom-configuration-sections/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BibleReading.Common45.Root.Web.License.Configuration
{
    public class LicenseGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("licenseSettings", IsRequired = true)]
        public LicenseSection LicenseSection
        {
            get { return (LicenseSection)base.Sections["licenseSettings"]; }
        }
    }
}
