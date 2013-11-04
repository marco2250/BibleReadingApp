using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace BibleReading.Common45.Root.Net
{
    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public bool CheckValidationResult(ServicePoint servicePoint
            , X509Certificate cert
            , WebRequest webRequest
            , int certProb)
        {
            return true;
        }
    }
}
