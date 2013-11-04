using System.Net;

using BibleReading.Common45.Properties;

namespace BibleReading.Common45.Root.Net
{
    public static class ProxyUtility
    {
        public static WebProxy GetProxy()
        {
            if (Settings.Default.ProxyEnabled && !Settings.Default.ProxyIP.IsNullOrEmpty())
            {
                var proxy = new WebProxy(Settings.Default.ProxyIP, Settings.Default.ProxyDoor);

                if (!Settings.Default.ProxyUser.IsNullOrEmpty())
                {
                    proxy.Credentials = new NetworkCredential(
                        Settings.Default.ProxyUser
                        , Settings.Default.ProxyPassword
                        , Settings.Default.ProxyDomain);
                }

                return proxy;
            }
            return null;
        }
    }
}
