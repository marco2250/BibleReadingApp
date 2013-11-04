using System;
using System.Linq;
using System.Data;
using System.Threading;
using System.Web;

namespace BibleReading.Common45.Root.Web.License
{
    public static class LicenseValidation
    {
        #region License Validation Result

        private const string LicenseNotFound = "LICENSE_NOT_FOUND";
        private const string LicenseTimeout = "LICENSE_TIMEOUT";

        private const string DoorStateUndefined = "DOOR_STATE_UNDEFINED";
        private const string DoorStateOK = "DOOR_STATE_OK";
        private const string DoorStateClosed = "DOOR_STATE_CLOSED";
        private const string DoorStateError = "DOOR_STATE_ERROR";

        private const string SessionExpired = "SESSION_EXPIRED";

        /// <summary>
        /// A licença já havia sido previamente liberada.
        /// Esta constante significa que as requisições para o usuário estão liberadas.
        /// </summary>
        private const string RequestOK = "REQUEST_OK";

        #endregion

        public static void Init(string request)
        {
            request = LicenseInfo.Instance.Crypter.DecryptString(request);
            var supportInfo = request.GetSupportInfo();

            // We try to load the license file on every request
            var strLicense = LicenseGear.GetLicense();

            if (string.IsNullOrEmpty(strLicense))
                return;

            // Fake Application_OnStart

            var ds = CryptDataSet.GetDataSet(strLicense);

            // Clears any active license for this server, thus it supports web farm
            var delete = false;
            ds.Tables["dtSessions"].Select("ServerName = '" + supportInfo["ServerName"] + "'").ToList().ForEach(
                delegate(DataRow rw)
                    {
                        delete = true;
                        rw.Delete();
                    });

            if (delete)
                LicenseGear.UpdateLicense(CryptDataSet.EncryptDataSet(ds));
        }

        public static string ValidateLicenseAccess(string request)
        {
            var lockObject = new object();

            lock (lockObject)
            {
                var ret = string.Empty;

                request = LicenseInfo.Instance.Crypter.DecryptString(request);
                var supportInfo = request.GetSupportInfo();

                // We try to load the licenses file on every request
                var strLicense = LicenseGear.GetLicense();

                // Check if we have the licensing file set up
                if (string.IsNullOrEmpty(strLicense))
                {
                    // We don't have the license file
                    ret += "&result=" + LicenseNotFound;
                }
                else
                {
                    var ds = CryptDataSet.GetDataSet(strLicense);

                    var itSelf = false;
                    ds.Tables["dtSessions"].Select().ToList().ForEach(
                            delegate(DataRow rw)
                            {
                                var lastSignal = (DateTime)rw["LastSignal"];
                                var signalBoundary = DateTime.Now.AddMilliseconds(-int.Parse(supportInfo["SessionRefreshTimeoutWithTolerance"]));

                                if (lastSignal >= signalBoundary)
                                {
                                }
                                else
                                {
                                    // Session has expired
                                    // User browser isn't at AgroERP

                                    if (rw["SessionID"].ToString() == supportInfo["SessionID"])
                                        itSelf = true;

                                    rw.Delete();
                                }
                            });
                    ds.AcceptChanges();

                    if (itSelf)
                    {
                        ret += "&result=" + LicenseTimeout;
                        LicenseGear.UpdateLicense(CryptDataSet.EncryptDataSet(ds));
                    }
                    else
                    {
                        if (!LicenseGear.IsAccessAllowed(ds, supportInfo["SessionID"]))
                        {
                            // Fake Session_OnStart

                            var doorState = DoorStateUndefined;
                            try
                            {
                                var numberOfLicenses = int.Parse(CryptDataSet.GetConfigurationValue(ds, "NumberOfLicenses"));
                                var activeLicenses = ds.Tables["dtSessions"].Rows.Count;

                                if (numberOfLicenses != -1)
                                {
                                    if (activeLicenses < numberOfLicenses)
                                    {
                                        // Increments one session
                                        var rw = ds.Tables["dtSessions"].NewRow();
                                        rw["ServerName"] = supportInfo["ServerName"];
                                        rw["SessionID"] = supportInfo["SessionID"];
                                        rw["LastSignal"] = DateTime.Now;
                                        ds.Tables["dtSessions"].Rows.Add(rw);

                                        doorState = DoorStateOK;
                                    }
                                    else
                                    {
                                        // No more licenses

                                        doorState = DoorStateClosed;
                                    }
                                }
                                else
                                {
                                    // Unlimited licenses

                                    doorState = DoorStateOK;
                                }

                                if (doorState == DoorStateOK)
                                {
                                    LicenseGear.UpdateLicense(CryptDataSet.EncryptDataSet(ds));
                                }
                            }
                            catch (ThreadAbortException)
                            {
                            }
                            catch (Exception ex)
                            {
                                // Error
                                // We cannot allow any user access the system

                                doorState = DoorStateError;

                                ret += "&errorMessage=" + ex.Message;
                                ret += "&stackTrace=" + ex;
                            }

                            ret += "&result=" + doorState;
                        }
                        else
                        {
                            var rwSession = ds.Tables["dtSessions"].Select("SessionID = '" + supportInfo["SessionID"] + "'").FirstOrDefault();
                            var lastRequest = DateTime.Now;

                            if (rwSession["LastRequest"] != DBNull.Value)
                                lastRequest = (DateTime)rwSession["LastRequest"];

                            var requestBoundary = DateTime.Now.AddMinutes(-int.Parse(supportInfo["SessionTimeout"]));

                            // Is It an "Human" Request?
                            // Yes

                            if (lastRequest < requestBoundary)
                            {
                                // Session has expired
                                // User browser isn't at AgroERP

                                rwSession.Delete();

                                ret += "&result=" + SessionExpired;
                            }
                            else
                            {
                                // Resets Real Request ticket
                                rwSession["LastRequest"] = DateTime.Now;

                                // Resets LastSignal ticket
                                rwSession["LastSignal"] = DateTime.Now;

                                ret += "&result=" + RequestOK;
                            }

                            LicenseGear.UpdateLicense(CryptDataSet.EncryptDataSet(ds));
                        }
                    }
                }

                return ret.Substring(1) + "&guid=" + Guid.NewGuid().ToString();
            }
        }

        public static void Logout(string serverName, string sessionID)
        {
            // We try to load the license file on every request
            var strLicense = LicenseGear.GetLicense();

            if (string.IsNullOrEmpty(strLicense))
                return;

            // Fake Application_OnStart

            var ds = CryptDataSet.GetDataSet(strLicense);

            // Clears any active license for this server, thus it supports web farm
            var delete = false;
            ds.Tables["dtSessions"].Select("ServerName = '" + serverName + "' AND SessionID = '" + sessionID + "'").ToList().ForEach(
                delegate(DataRow rw)
                {
                    delete = true;
                    rw.Delete();
                });

            if (delete)
                LicenseGear.UpdateLicense(CryptDataSet.EncryptDataSet(ds));
        }

        public static void TickSignal(string sessionID)
        {
            var obj = new object();

            try
            {
                lock (obj)
                {
                    sessionID = HttpContext.Current.Server.UrlDecode(sessionID);

                    var strLicense = LicenseGear.GetLicense();
                    var ds = CryptDataSet.GetDataSet(strLicense);

                    sessionID = LicenseInfo.Instance.Crypter.DecryptString(sessionID);

                    var rwSession = ds.Tables["dtSessions"].Select("SessionID = '" + sessionID + "'").FirstOrDefault();

                    if (rwSession != null)
                    {
                        rwSession["LastSignal"] = DateTime.Now;
                        LicenseGear.UpdateLicense(CryptDataSet.EncryptDataSet(ds));
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Trace.Warn(ex.ToString());

                throw;
            }
        }
    }
}
