using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.OleDb;

namespace BibleReading.Common45.Root.MSOffice
{
    public class ExcelUtility
    {
        public static DataTable ImportToDataTable(string file)
        {
            string strConn = string.Empty;

            FileInfo fl = new FileInfo(file);

            if (fl.Extension.ToUpper() == ".XLS")
                strConn = "Provider=Microsoft.Jet.OleDb.4.0;" +
                    "Data Source=" + file + ";Extended Properties=Excel 8.0;";
            else
                strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" +
                    "Data Source=" + file + ";Extended Properties=Excel 12.0;";


            DataTable dataTable = new DataTable();
            using (OleDbConnection objConn = new OleDbConnection(strConn))
            {
                objConn.Open();

                DataTable dtSchema = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string table = dtSchema.Rows[1]["TABLE_NAME"].ToString();

                string strSql = "Select * From [" + table + "]";
                OleDbCommand objCmd = new OleDbCommand(strSql, objConn);

                OleDbDataAdapter adapter = new OleDbDataAdapter(strSql, objConn);
                adapter.Fill(dataTable);
            }

            return dataTable;
        }
    }
}
