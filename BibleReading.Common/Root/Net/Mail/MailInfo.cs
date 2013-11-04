using System.Configuration;

using BibleReading.Common45.Properties;

namespace BibleReading.Common45.Root.Net.Mail
{
    public static class MailInfo
    {
        public static SendMail.ExchangeSmtp ServerMode
        {
            get { return (SendMail.ExchangeSmtp)Settings.Default.Mail_SendUsing; }
        }

        public static string Domain
        {
            get { return Settings.Default.Mail_Domain; }
        }

        public static string User
        {
            get { return Settings.Default.Mail_User; }
        }

        public static string Password
        {
            get { return Settings.Default.Mail_Password; }
        }

        public static string Server
        {
            get { return Settings.Default.Mail_Server; }
        }

        public static int Port
        {
            get { return Settings.Default.Mail_Port; }
        }

        public static bool Ssl
        {
            get { return Settings.Default.Mail_Ssl; }
        }

        public static bool AsyncEnabled
        {
            get { return Settings.Default.Mail_AsyncEnabled; }
        }

        public static string From
        {
            get { return Settings.Default.Mail_From_Email; }
        }

        public static string FromName
        {
            get { return Settings.Default.Mail_From_Name; }
        }
    }
}
