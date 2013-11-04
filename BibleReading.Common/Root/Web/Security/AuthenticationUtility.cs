using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace BibleReading.Common45.Root.Web.Security
{
    public class AuthenticationUtility
    {
        public static AuthenticationMode GetAuthenticationMode()
        {
            AuthenticationSection section = (AuthenticationSection)WebConfigurationManager.GetSection("system.web/authentication");

            return section.Mode;
        }
    }
}
