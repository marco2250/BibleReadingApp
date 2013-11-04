using System.Web;

namespace BibleReading.Common45.Root.Web
{
    public static class HttpContextExtension
    {
        public static string GetCurrentUrl(this HttpContext context)
        {
            var url = context.Request.ServerVariables["URL"];
            if (!context.Request.ServerVariables["QUERY_STRING"].IsNullOrEmpty())
                url += "?" + context.Request.ServerVariables["QUERY_STRING"];

            if (url.StartsWith("/"))
                url = url.Substring(1);

            if(context.Request.ServerVariables["SERVER_PORT"] == "80")
                url = context.Request.ServerVariables["SERVER_NAME"] + "/" + url;
            else
                url = context.Request.ServerVariables["SERVER_NAME"] + ":" + context.Request.ServerVariables["SERVER_PORT"] + "/" + url;

            if (context.Request.ServerVariables["HTTPS"].ToUpper() == "ON")
                url = "https://" + url;
            else
                url = "http://" + url;

            return url;
        }
    }
}
