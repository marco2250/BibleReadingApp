using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Web;

namespace BibleReading.Common45.Root.Security.Cryptography
{
    public class CryptUtil
    {
        public byte[] LicenseKey { get; set; }
        public byte[] LicenseVector { get; set; }

        private SymmetricCryptography<TripleDESCryptoServiceProvider> _CryptObject;
        public SymmetricCryptography<TripleDESCryptoServiceProvider> CryptObject
        {
            get
            {
                if (this._CryptObject == null)
                    this._CryptObject = new SymmetricCryptography<TripleDESCryptoServiceProvider>(this.LicenseKey, this.LicenseVector);

                return this._CryptObject;
            }
        }

        private CryptUtil()
        {
        }

        public CryptUtil(byte[] licenseKey, byte[] licenseVector)
        {
            this.LicenseKey = licenseKey;
            this.LicenseVector = licenseVector;
        }

        public string EncryptString(string s)
        {
            if(!string.IsNullOrEmpty(s))
                return this.CryptObject.Encrypt(s);
            else
                return null;
        }

        public string DecryptString(string s)
        {
            return this.CryptObject.Decrypt(s);
        }

        public static string DecryptString(string s, byte[] bytKey, byte[] bytVector)
        {
            SymmetricCryptography<TripleDESCryptoServiceProvider> sc = new SymmetricCryptography<TripleDESCryptoServiceProvider>(bytKey, bytVector);
            return sc.Decrypt(s);
        }

        public string DecryptString(Stream stream)
        {
            return CryptUtil.DecryptString(stream, this.LicenseKey, this.LicenseVector);
        }

        public static string DecryptString(Stream stream, byte[] bytKey, byte[] bytVector)
        {
            SymmetricCryptography<TripleDESCryptoServiceProvider> sc = new SymmetricCryptography<TripleDESCryptoServiceProvider>(bytKey, bytVector);

            CryptoStream cStream = new CryptoStream(stream, sc._provider.CreateDecryptor(sc.Key, sc.IV), CryptoStreamMode.Read);

            byte[] bytLicense = null;
            int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int bytesRead;
            int pos = 0;
            int totBytRead = 0;

            do
            {
                // read a chunk of data from the input file
                bytesRead = cStream.Read(buffer, 0, bufferLen);
                totBytRead += bytesRead;

                if (bytesRead > 0)
                {
                    byte[] aux = bytLicense;

                    bytLicense = new byte[totBytRead];

                    if (aux != null)
                        aux.CopyTo(bytLicense, 0);

                    Array.Copy(buffer, 0, bytLicense, pos, bytesRead);

                    pos += bytesRead;
                }
            }
            while (bytesRead != 0);

            cStream.Close();
            cStream.Dispose();

            return ASCIIEncoding.ASCII.GetString(bytLicense);
        }
    }
}
