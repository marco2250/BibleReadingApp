using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

using BibleReading.Common45.Root.Data.SqlClient.DataRowFW;

namespace BibleReading.Common45.Root.Data.SqlClient
{
    public class UtilDB : BaseDB
    {
        public DataTable GetReferences(string tableName, string keyValue)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(this.GetSqlParameter<string>("V_TABLE_NAME", tableName));
            parameters.Add(this.GetSqlParameter<string>("V_PK1_VALUE", keyValue));

            return this.GetDataTable("dbo.spCheckReferences", parameters, "Reference");
        }

        public bool CheckUnique(string uniqueKey, string values, string pkValue)
        {
            IList<SqlParameter> parameters = new List<SqlParameter>();

            parameters.Add(this.GetSqlParameter<string>("UNIQUE_NAME", uniqueKey));
            parameters.Add(this.GetSqlParameter<string>("VALUES", values));

            if (!pkValue.IsNullOrEmpty())
                parameters.Add(this.GetSqlParameter<string>("PK_VALUE", pkValue));

            return this.GetScalar<string>("dbo.spCheckUnique", parameters) == "Yes";
        }

        public static bool IsDatabaseAlive(string connectionString)
        {
            SqlConnection cnn = null;
            try
            {
                cnn = new SqlConnection(connectionString);
                cnn.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if (cnn != null && cnn.State != ConnectionState.Closed)
                {
                    try
                    {
                        cnn.Close();
                        cnn.Dispose();
                    }
                    catch { }
                }
            }
        }
    }
}
