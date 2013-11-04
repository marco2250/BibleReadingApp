using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

using BibleReading.Common45.Root.Net;
using BibleReading.Common45.Root.Security.Cryptography;
using BibleReading.Common45.Properties;
using BibleReading.Common45.Root.Web.License.Configuration;

namespace BibleReading.Common45.Root.Web.License
{
    public class LicenseInfo
    {
        // http://msdn.microsoft.com/en-us/library/ff650316.aspx

        private static object syncRoot = new object();

        private static volatile LicenseInfo instance;
        public static LicenseInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new LicenseInfo();

                            ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();
                        }
                    }
                }

                return instance;
            }
        }

        public byte[] LicenseKey
        {
            get
            {
                // License key/fector for agrione
                //return new byte[24] { 185, 146, 35, 173, 254, 35, 26, 125, 161, 134, 233, 137, 239, 223, 9, 119, 78, 140, 78, 236, 65, 85, 228, 124 };

                // License key/vector for CGG Trading S.A.
                return new byte[24] { 108, 4, 10, 122, 18, 16, 149, 15, 139, 133, 185, 181, 60, 119, 230, 148, 72, 74, 36, 28, 51, 137, 71, 189 };

                // License key/vector for Multigrain S.A.
                //return new byte[24] { 245, 166, 181, 115, 185, 223, 188, 116, 193, 142, 27, 93, 123, 255, 241, 228, 18, 54, 147, 96, 81, 252, 143, 96 };

                // License key/vector for Terlogs Terminal Marítimio
                //return new byte[24] { 229, 25, 85, 208, 199, 223, 143, 24, 26, 201, 195, 102, 63, 156, 236, 218, 0, 183, 58, 31, 254, 64, 62, 129 };
            }
        }

        public byte[] LicenseVector
        {
            get
            {
                // License key/fector for agrione
                //return new byte[8] { 192, 43, 156, 213, 113, 25, 162, 70 };

                // License key/vector for CGG Trading S.A.
                return new byte[8] { 115, 156, 131, 162, 133, 5, 30, 216 };

                // License key/vector for Multigrain S.A.
                //return new byte[8] { 253, 63, 45, 155, 44, 213, 68, 61 };

                // License key/vector for Terlogs Terminal Marítimio
                //return new byte[8] { 236, 177, 205, 249, 58, 213, 23, 225 };
            }
        }

        private CryptUtil _Crypter;
        public CryptUtil Crypter
        {
            get
            {
                if (this._Crypter == null)
                    this._Crypter = new CryptUtil(this.LicenseKey, this.LicenseVector);

                return this._Crypter;
            }
        }

        public string ServerName
        {
            get
            {
                return ConfigurationHelper.GetCurrentComponentConfiguration().LicenseSection.ServerName;
            }
        }

        public int SessionRefreshTime
        {
            get
            {
                try
                {
                    return Settings.Default.License_SessionRefreshTime * 1000;
                }
                catch (Exception ex)
                {
                    return 30;
                }
            }
        }

        public int SessionRefreshTimeout
        {
            get
            {
                try
                {
                    return Settings.Default.License_SessionRefreshTimeout * 1000;
                }
                catch (Exception ex)
                {
                    return 60;
                }
            }
        }

        public int SessionRefreshTimeoutTolerance
        {
            get
            {
                try
                {
                    return Settings.Default.License_SessionRefreshTimeoutTolerance;
                }
                catch (Exception ex)
                {
                    return 20;
                }
            }
        }

        /// <summary>
        /// Tempo, em milisegundos, com tolerancia, no qual o usuário deve enviar o sinal que está "alive".
        /// </summary>
        public int SessionRefreshTimeoutWithTolerance
        {
            get
            {
                return (int)(((this.SessionRefreshTimeoutTolerance * 0.01) + 1) * this.SessionRefreshTimeout);
            }
        }
    }
}
