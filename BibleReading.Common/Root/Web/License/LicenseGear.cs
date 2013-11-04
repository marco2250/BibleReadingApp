using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using BibleReading.Common45.Root.Data.SqlClient.DataRowFW;
using BibleReading.Common45.Properties;

namespace BibleReading.Common45.Root.Web.License
{
    public class LicenseGear
    {
        private static readonly BaseDB BaseDB = new BaseDB(Settings.Default.License_ConnectionString);

        public enum LicenseMode
        {
            RegisteredUsers = 1,
            ActiveSessions = 2
        }

        public static string GetLicense()
        {
            return BaseDB.GetScalar<string>("spLicensingGet");
        }

        internal static void UpdateLicense(string licenseData)
        {
            BaseDB.ExecuteNonQuery("spLicensingUpdate", "dsc_licensing", licenseData);
        }

        public static void DeleteTicket(string sessionID)
        {
            var strLicense = GetLicense();

            var ds = CryptDataSet.GetDataSet(strLicense);
            var rw = ds.Tables["dtSessions"].AsEnumerable().ToList().FirstOrDefault(x => x.Field<string>("SessionID") == sessionID);

            if (rw == null)
                return;

            rw.Delete();
            ds.AcceptChanges();

            UpdateLicense(CryptDataSet.EncryptDataSet(ds));
        }

        public static List<string> GetAllowedPages()
        {
            if (HttpContext.Current.Application["ap"] == null)
                return null;

            return (List<string>)HttpContext.Current.Application["ap"];
        }

        public static void EncryptAllowedPages()
        {
            var allowedPages = (List<string>)HttpContext.Current.Application["ap"];

            for (int i = 0; i < allowedPages.Count; i++)
                allowedPages[i] = LicenseInfo.Instance.Crypter.EncryptString(allowedPages[i]);
        }

        public static bool IsAllowedPage()
        {
            return GetAllowedPages().IndexOf(LicenseInfo.Instance.Crypter.EncryptString(HttpContext.Current.Handler.GetType().Name.ToLower())) > -1;
        }

        public static bool IsAccessAllowed(DataSet ds, string sessionID)
        {
            return ds.Tables["dtSessions"].Select("SessionID = '" + sessionID + "'").Length > 0;
        }

        public static LicenseMode GetLicenseMode()
        {
            var strLicense = GetLicense();
            var ds = CryptDataSet.GetDataSet(strLicense);

            if (ds == null)
                throw new Exception("Invalid license information");

            return (LicenseMode)Convert.ToInt32(CryptDataSet.GetConfigurationValue(ds, "LicenseMode"));
        }

        public static bool IsLoggedIn(DataSet ds, string userName)
        {
            return ds.Tables["dtSessions"].Select("UserName = '" + userName + "'").Length > 0;
        }

        public enum CheckLicenseStatus
        {
            Valid = 1,
            Invalid = 2
        }

        public enum LicenseFileStatus
        {
            Missing = 1,
            Valid = 2,
            Invalid = 3
        }

        public static CheckLicenseStatus CheckUserLicense()
        {
            // 'This is a double check
            // 'We check if user has access to the system (if user got a license)

            var objLock = new object();

            lock (objLock)
            {
                if (CheckLicenseFile() != LicenseFileStatus.Valid)
                    throw new Exception("Invalid License File");

                var strLicense = GetLicense();
                var ds = CryptDataSet.GetDataSet(strLicense);

                int numberOfLicenses = int.Parse(CryptDataSet.GetConfigurationValue(ds, "NumberOfLicenses"));
                if (numberOfLicenses == -1 || IsAccessAllowed(ds, HttpContext.Current.Session.SessionID))
                    return CheckLicenseStatus.Valid;

                return CheckLicenseStatus.Invalid;
            }
        }

        public static LicenseFileStatus CheckLicenseFile()
        {
            try
            {
                var strLicense = GetLicense();

                if (strLicense.IsNullOrEmpty() || CryptDataSet.GetDataSet(strLicense) == null)
                    return LicenseFileStatus.Invalid;

                return LicenseFileStatus.Valid;
            }
            catch
            {
                return LicenseFileStatus.Invalid;
            }
        }

        public static int GetNumberOfLicenses()
        {
            var strLicense = GetLicense();
            var ds = CryptDataSet.GetDataSet(strLicense);

            var numberOfLicenses = int.Parse(CryptDataSet.GetConfigurationValue(ds, "NumberOfLicenses"));
            return numberOfLicenses;
        }

        public static void TickSignal()
        {
            var obj = new object();

            lock (obj)
            {
                string strLicense = GetLicense();
                DataSet ds = CryptDataSet.GetDataSet(strLicense);

                DataRow rwSession = ds.Tables["dtSessions"].Select("SessionID = \'" + HttpContext.Current.Session.SessionID + "\'").FirstOrDefault();

                rwSession["LastSignal"] = DateTime.Now;

                UpdateLicense(CryptDataSet.EncryptDataSet(ds));
            }
        }
    }
}
