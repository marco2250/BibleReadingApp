using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Data;
using System.Web.UI;

using BibleReading.Common45.Root.Net.Mail;
using BibleReading.Common45.Properties;
using BibleReading.Common45.Root.Web.License.Configuration;

namespace BibleReading.Common45.Root.Web.License
{
    public class LicenseModule : IHttpModule
    {
        #region License Validation Result

        private const string LicenseTimeout = "LICENSE_TIMEOUT";

        private const string DoorStateOK = "DOOR_STATE_OK";
        private const string DoorStateClosed = "DOOR_STATE_CLOSED";
        private const string DoorStateError = "DOOR_STATE_ERROR";

        private const string SessionExpired = "SESSION_EXPIRED";

        private const string RequestOK = "REQUEST_OK";

        #endregion

        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.PostAcquireRequestState += context_PostAcquireRequestState;
        }

        private void context_PostAcquireRequestState(object sender, EventArgs e)
        {
            HttpContext.Current.Trace.Warn("Warn 0");

            var app = ((HttpApplication)sender);

            try
            {
                var lockObject = new object();

                lock (lockObject)
                {
                    HttpContext.Current.Trace.Warn("Warn 1");

                    if (LicenseGear.GetAllowedPages() == null)
                    {
                        HttpContext.Current.Trace.Warn("Warn 2");

                        #region Allowed Pages

                        IList<string> allowedPages = new List<string>();
                        //allowedPages.Add("generalparameters_licensing_frmlicensingdetail_aspx");
                        allowedPages.Add("opened_frmaccessdenied_aspx");
                        allowedPages.Add("opened_frmerrorpage_aspx");
                        allowedPages.Add("opened_frmerrorsearch_aspx");
                        allowedPages.Add("opened_frmlicenseerror_aspx");
                        allowedPages.Add("opened_frmlicenselimitreached_aspx");
                        allowedPages.Add("opened_frmlicensetimeout_aspx");
                        allowedPages.Add("opened_frmsessionout_aspx");
                        allowedPages.Add("frmlogin_aspx");

                        app.Application["ap"] = allowedPages;

                        HttpContext.Current.Trace.Warn("Warn 3");

                        LicenseGear.EncryptAllowedPages();

                        HttpContext.Current.Trace.Warn("Warn 4");

                        #endregion
                    }
                }

                HttpContext.Current.Trace.Warn("Warn 5");

                if (!app.Context.Handler.GetType().Name.ToLower().EndsWith("_aspx"))
                    return;

                if (LicenseGear.IsAllowedPage())
                {
                    return;
                }

                if (LicenseGear.CheckLicenseFile() != LicenseGear.LicenseFileStatus.Valid)
                    app.Response.Redirect("~/Opened/FrmLicenseError.aspx", false);

                HttpContext.Current.Trace.Warn("Warn 6");

                lock (lockObject)
                {
                    HttpContext.Current.Trace.Warn("Warn 7");

                    // We try to load the licenses file on every request
                    string strLicense = LicenseGear.GetLicense();
                    HttpContext.Current.Trace.Warn("Warn 8");

                    if (string.IsNullOrEmpty(strLicense))
                    {
                        app.Response.Redirect("~/GeneralParameters/Licensing/FrmLicensingDetail.aspx", false);
                    }

                    DataSet ds = CryptDataSet.GetDataSet(strLicense);

                    int numberOfLicenses = int.Parse(CryptDataSet.GetConfigurationValue(ds, "NumberOfLicenses"));
                    HttpContext.Current.Trace.Warn("Warn 11");
                    if (numberOfLicenses == -1)
                    {
                        // Licenças ilimitadas

                        HttpContext.Current.Trace.Warn("Warn 12");

                        if (app.Context.Handler.GetType().Name.ToLower() == "default_aspx")
                        {
                            HttpContext.Current.Trace.Warn("Warn 13");

                            //Remoção por solicitação AnaCélia / Athos - 15/08/2013
                            //if (!((Page)app.Context.Handler).IsPostBack
                            //    && app.Request["admin"] == "1"
                            //    && app.Request["pwd"] == "AgroERPAdminUser")
                            //{
                            //    HttpContext.Current.Session["la"] = 1;
                            //}

                            HttpContext.Current.Trace.Warn("Warn 15");
                        }

                        HttpContext.Current.Trace.Warn("Warn 16");

                        return;
                    }

                    string request;
                    if (LicenseGear.GetLicenseMode() == LicenseGear.LicenseMode.ActiveSessions)
                    {
                        if (app.Application["Init"] == null)
                        {
                            request = string.Empty;
                            request += "&ServerName=" + LicenseInfo.Instance.ServerName;

                            new wsLicense.License().Init(LicenseInfo.Instance.Crypter.EncryptString(request.Substring(1)));

                            HttpContext.Current.Trace.Warn("Warn 20");

                            app.Application["Init"] = true;
                        }
                    }

                    //Remoção por solicitação AnaCélia / Athos - 15/08/2013
                    // A linha abaixo deve ser guardada a 7 chaves!!!
                    //if (!((Page)app.Context.Handler).IsPostBack
                    //    && app.Request["admin"] == "1"
                    //    && app.Request["pwd"] == "AgroERPAdminUser")
                    //{
                    //    HttpContext.Current.Session["la"] = 1;

                    //    app.Response.Redirect("~/Default.aspx");
                    //}

                    if (LicenseGear.GetLicenseMode() == LicenseGear.LicenseMode.ActiveSessions)
                    {
                        request = string.Empty;
                        request += "&SessionID=" + app.Session.SessionID;
                        request += "&SessionTimeout=" + app.Session.Timeout;
                        request += "&ServerName=" + LicenseInfo.Instance.ServerName;
                        request += "&SessionRefreshTimeoutWithTolerance=" + LicenseInfo.Instance.SessionRefreshTimeoutWithTolerance;

                        var supportInfo = new wsLicense.License().ValidateLicenseAccess(LicenseInfo.Instance.Crypter.EncryptString(request.Substring(1)));
                        var response = supportInfo.GetSupportInfo();

                        switch (response["result"])
                        {
                            case LicenseTimeout:
                                {
                                    app.Response.Redirect("~/Opened/FrmLicenseTimeout.aspx", false);

                                    break;
                                }
                            case DoorStateOK:
                                {
                                    //app.Response.Redirect("~/Default.aspx", false);
                                    break;
                                }
                            case DoorStateClosed:
                                {
                                    app.Response.Redirect("~/Opened/FrmLicenseLimitReached.aspx", false);

                                    break;
                                }
                            case DoorStateError:
                                {
                                    try
                                    {
                                        string body = "<html xmlns=\"http://www.w3.org/1999/xhtml\">" +
                                        "<head>" +
                                        "<title></title>" +
                                        "</head>" +
                                        "<body style=\"font-size: 10pt; font-family: Arial\">" +
                                        "Dear All, " +
                                        "<br />" +
                                        "<br />" +
                                        " An error has happened in the system while validating license." +
                                            "<br />" +
                                            "<br />" +
                                            "<b>Date:</b> #dt_error#" +
                                            "<br />" +
                                            "<b>Error Message:</b> #dsc_error_message#" +
                                            "<br />" +
                                            "<b>Url:</b> #dsc_url#" +
                                            "<br />" +
                                            "<br />" +
                                            "<b>Stack Trace:</b>" +
                                            "<br />" +
                                            "#dsc_inner_error#" +
                                            "<br />" +
                                            "-- " +
                                            "<br />" +
                                            "<br />" +
                                        "Sent by <b>#dsc_username#</b> via AgroERP System" +
                                        "</body>" +
                                        "</html>";

                                        body = body.Replace("#dt_error#", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                                        body = body.Replace("#dsc_error_message#", response["errorMessage"]);
                                        body = body.Replace("#dsc_inner_error#", response["stackTrace"]);

                                        new SendMail().Send(Settings.Default.Mail_User
                                            , Settings.Default.Mail_Password
                                            , Settings.Default.Mail_Domain
                                            , Settings.Default.Mail_Server
                                            , Settings.Default.Mail_Port.ToString()
                                            , Settings.Default.Mail_Ssl
                                            , (SendMail.ExchangeSmtp)Settings.Default.Mail_SendUsing
                                            , Settings.Default.Mail_From_Email
                                            , Settings.Default.Mail_From_Name
                                            , Settings.Default.License_BugEmail
                                            , Settings.Default.License_BugSubject
                                            , new StringBuilder(body)
                                            , true
                                            , System.Net.Mail.MailPriority.High
                                            , Settings.Default.Mail_AsyncEnabled
                                            , false
                                            , null);
                                    }
                                    catch (Exception) { }

                                    app.Response.Redirect("~/Opened/FrmLicenseError.aspx", false);

                                    break;
                                }
                            case RequestOK:
                                {
                                    // A licença já havia sido previamente liberada.
                                    // Esta constante significa que as requisições para o usuário estão liberadas.

                                    break;
                                }
                            case SessionExpired:
                                {
                                    app.Response.Redirect("~/Opened/FrmSessionOut.aspx", false);

                                    break;
                                }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                app.Response.Redirect("~/Opened/FrmLicenseError.aspx", false);
            }
        }

        #endregion
    }
}
