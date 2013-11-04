/*
 * http://msdn.microsoft.com/en-us/library/ms526318(EXCHG.10).aspx
 * 
 * Schema: http://schemas.microsoft.com/cdo/configuration/
 */

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.ComponentModel;
using System.Web.UI;

namespace BibleReading.Common45.Root.Net.Mail
{
    public class SendMail
    {
        public enum ExchangeSmtp
        {
            Exchange = 1,
            Smtp = 2
        }

        public void NewCc(string cc)
        {
            _mail.CC.Add(new MailAddress(cc));
        }

        public void NewAttachment(MemoryStream attachment, string name)
        {
            var mime = string.Empty;

            switch(new FileInfo(name).Extension.ToLower())
            {
                case ".pdf":
                    {
                        mime = "application/pdf";
                        break;
                    }
            }

            _mail.Attachments.Add(new Attachment(attachment, name, mime));
        }

        readonly MailMessage _mail = new MailMessage();
        public void Send(string userName
            , string password
            , string domain
            , string server
            , string port
            , bool ssl
            , ExchangeSmtp serverMode
            , string from
            , string fromName
            , string toEmail
            , string subject
            , StringBuilder body
            , bool html
            , MailPriority priority
            , bool asyncEnabled
            , bool sendAsync
            , List<Pair> bcc)
        {
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            _mail.From = new MailAddress(from, fromName);
            _mail.Subject = subject;
            _mail.Body = body.ToString();
            _mail.IsBodyHtml = html;
            _mail.Priority = priority;

            if (toEmail.Contains(";"))
            {
                foreach (string to in toEmail.Split(';'))
                {
                    _mail.To.Add(new MailAddress(to));
                }
            }
            else
            {
                _mail.To.Add(new MailAddress(toEmail));
            }

            if (bcc != null)
            {
                (from bccMail in bcc select bccMail).ToList().ForEach(
                    bccMail => _mail.Bcc.Add(new MailAddress(bccMail.First.ToString(), bccMail.Second.ToString())));
            }

            var smtpClient = new SmtpClient
                                 {
                                     Credentials = new NetworkCredential(userName, password, domain),
                                     Host = server,
                                     Port = int.Parse(port),
                                     EnableSsl = ssl
                                 };

            if (!sendAsync || !asyncEnabled)
                smtpClient.Send(_mail);
            else
            {
                smtpClient.SendCompleted += smtpClient_SendCompleted;
                smtpClient.SendAsync(_mail, null);
            }
        }

        private void smtpClient_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
        }
    }
}
