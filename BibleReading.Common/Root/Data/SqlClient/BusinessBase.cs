using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BibleReading.Common45.Root.Data.SqlClient
{
    public class BusinessBase
    {
        protected readonly string SPLITTER = ((char)215).ToString() + ((char)252).ToString();

        public DataTable GetReferences(string tableName, string keyValue)
        {
            return new UtilDB().GetReferences(tableName, keyValue);
        }

        public bool CheckReferences(string tableName, string key, ref string errorMessages)
        {
            DataTable dtReferences = this.GetReferences(tableName, key);

            if (dtReferences.Rows.Count > 0)
            {
                DataRow[] rows = dtReferences.Select("quantity > 0");

                StringBuilder messages = new StringBuilder();

                foreach (DataRow dr in rows)
                    //messages.Append(Resources.DeleteErrorMessages.ResourceManager.GetString(dr["foreign_table"].ToString()) + "<br />");

                errorMessages = messages.ToString();
                return rows.Length > 0;
            }
            else
            {
                return false;
            }
        }

        public bool CheckUnique(string uniqueKey, string values, string pkValue)
        {
            return new UtilDB().CheckUnique(uniqueKey, values, pkValue);
        }
    }
}
