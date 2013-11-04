using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Reporting.WinForms;

namespace BibleReading.Common45.Root.Reports.ReportViewer.Windows
{
    public class ReportViewerWinUtility
    {
        public static byte[] GeneratePdf(string path, string dataSourceName, string connectionString, string procedure, IList<SqlParameter> parameters)
        {
            return GenerateReport("PDF", path, dataSourceName, connectionString, procedure, parameters);
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
                rv.LocalReport.EnableExternalImages = true;
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
