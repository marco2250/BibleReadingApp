using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Security.Cryptography;

using BibleReading.Common45.Root.Security.Cryptography;

namespace BibleReading.Common45.Root.Web.License
{
    public class CryptDataSet
    {
        public void SaveToFile(string s, string fileName, byte[] bytKey, byte[] bytVector)
        {
            SymmetricCryptography<TripleDESCryptoServiceProvider> CryptObject = new SymmetricCryptography<TripleDESCryptoServiceProvider>(bytKey, bytVector);

            FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            CryptoStream cStream = new CryptoStream(fs, CryptObject._provider.CreateEncryptor(CryptObject.Key, CryptObject.IV), CryptoStreamMode.Write);

            byte[] bytLicense = ASCIIEncoding.ASCII.GetBytes(s);

            //Read from the input file, then encrypt and write to the output file.
            for (int i = 0; i < bytLicense.Length; i++)
                cStream.Write(bytLicense, i, 1);

            cStream.Close();
            cStream.Dispose();

            fs.Close();
            fs.Dispose();
        }

        public static DataSet GetDataSet(string s)
        {
            // Decrypts license
            string strLicense = LicenseInfo.Instance.Crypter.DecryptString(s);

            byte[] bytLicense = ASCIIEncoding.ASCII.GetBytes(strLicense);
            MemoryStream ms = new MemoryStream();
            ms.Write(bytLicense, 0, bytLicense.Length);
            ms.Position = 0;

            DataSet ds = new DataSet();
            ds.ReadXml(ms, XmlReadMode.ReadSchema);

            ms.Close();
            ms.Dispose();

            return ds;
        }

        public static string EncryptDataSet(DataSet ds)
        {
            StringWriter strWriter = new StringWriter();
            ds.WriteXml(strWriter, XmlWriteMode.WriteSchema);

            return LicenseInfo.Instance.Crypter.EncryptString(strWriter.GetStringBuilder().ToString());
        }

        public static DataSet DecryptFromFile(string fileName, byte[] bytKey, byte[] bytVector)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);

            try
            {
                DataSet ds = DecryptFromStream(fs);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                fs.Close();
                fs.Dispose();
            }
        }

        public static DataSet DecryptFromStream(Stream stream)
        {
            string s = LicenseInfo.Instance.Crypter.DecryptString(stream);
            return GetDataSet(s);
        }

        public static DataRow GetConfigurationRow(DataSet ds, string key)
        {
            return ds.Tables["dtConfiguration"].Select("Key = '" + key + "'").FirstOrDefault();
        }

        public static string GetConfigurationValue(DataSet ds, string key)
        {
            return GetConfigurationRow(ds, key)["Value"].ToString();
        }

        public static void UpdateConfiguration<T>(DataSet ds, string key, T value)
        {
            GetConfigurationRow(ds, key)["Value"] = value;
        }
    }
}
