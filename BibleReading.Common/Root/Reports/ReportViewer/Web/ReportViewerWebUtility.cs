using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Reporting.WinForms;

using WebReporting = Microsoft.Reporting.WebForms;

namespace BibleReading.Common45.Root.Reports.ReportViewer.Web
{
    public class ReportViewerWebUtility
    {
        public static byte[] GeneratePdf(string path, string dataSourceName, string connectionString, string procedure, IList<SqlParameter> parameters)
        {
            return GenerateReport("PDF", path, dataSourceName, connectionString, procedure, parameters);
        }

        public static byte[] GeneratePdf(string report, string server, IList<WebReporting.ReportParameter> parameters)
        {
            string mimeType;
            string encoding;
            string[] streamids;
            WebReporting.Warning[] warnings;
            var extension = "pdf";

            WebReporting.ReportViewer rpv = new WebReporting.ReportViewer();

            rpv.ProcessingMode = WebReporting.ProcessingMode.Remote;
            rpv.ServerReport.ReportPath = report;
            rpv.ServerReport.ReportServerUrl = new Uri(server);

            rpv.ServerReport.SetParameters((IEnumerable<WebReporting.ReportParameter>)parameters);

            byte[] bytes = rpv.ServerReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);

            return bytes;
        }

        public static byte[] GenerateReport(string type
            , string path
            , string dataSourceName
            , string connectionString
            , string procedure
            , IList<SqlParameter> parameters)
        {
            var cnn = new SqlConnection(connectionString);
            var cmd = new SqlCommand(procedure, cnn) { CommandType = CommandType.StoredProcedure };

            if (parameters != null)
                cmd.Parameters.AddRange(parameters.ToArray());

            var ds = new DataSet(dataSourceName);

            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                var rv = new Microsoft.Reporting.WinForms.ReportViewer {ProcessingMode = ProcessingMode.Local};

                var datasource = new ReportDataSource(dataSourceName, ds.Tables[0]);

                rv.LocalReport.ReportPath = path;
                rv.LocalReport.DataSources.Clear();
                rv.LocalReport.DataSources.Add(datasource);
                rv.LocalReport.EnableHyperlinks = true;
                rv.LocalReport.Refresh();

                string mimeType;
                string encoding;
                string filenameExtension;
                string[] streamids;
                Warning[] warnings;

                var streamBytes = rv.LocalReport.Render(type, null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);

                return streamBytes;
            }

            return null;
        }
    }
}
