using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using BibleReading.Common45.Properties;

namespace BibleReading.Common45.Root.Data.SqlClient.DataRowFW
{
    public class BaseDB
    {
        private SqlConnection _connection;
        public SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = new SqlConnection();

                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        private bool _ConnectionAutoClose = true;
        private bool ConnectionAutoClose
        {
            get { return _ConnectionAutoClose; }
            set { _ConnectionAutoClose = value; }
        }

        public string ConnectionString { get; set; }

        public BaseDB()
        {
            Connection = new SqlConnection(Settings.Default.DefaultConnectionString);
        }

        ~BaseDB()
        {
            CloseConnection();
        }

        public BaseDB(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private void OpenConnection()
        {
            if (Connection != null && Connection.State != ConnectionState.Open)
            {
                Connection.ConnectionString = ConnectionString;
                Connection.Open();
            }
        }

        private void CloseConnection()
        {
            try
            {
                if (Connection != null && Connection.State != ConnectionState.Closed)
                    Connection.Close();

                if (Connection != null)
                    Connection.Dispose();
            }
            catch { }
        }

        public SqlParameter GetSqlParameter<T>(string name, T value)
        {
            return new SqlParameter("@" + name, value);
        }

        #region GetScalar Methods

        public T GetScalar<T>(string procedureName)
        {
            return GetScalar<T>(procedureName, null);
        }

        public T GetScalar<T>(string procedureName, IList<SqlParameter> parameters)
        {
            lock (Connection)
            {
                try
                {
                    var cmd = new SqlCommand(procedureName, Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    OpenConnection();

                    object ret = cmd.ExecuteScalar();
                    if (ret == DBNull.Value)
                        return default(T);
                    else
                        return (T)Convert.ChangeType(ret, typeof(T));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (ConnectionAutoClose)
                        CloseConnection();
                }
            }
        }

        #endregion

        #region GetDataTable Methods

        public DataTable GetDataTable<T>(string procedureName
            , string parameterName
            , T parameterValue
            , string tableName)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetSqlParameter<T>(parameterName, parameterValue));

            return GetDataTable(procedureName, parameters, tableName);
        }

        public DataTable GetDataTable(string procedureName, IList<SqlParameter> parameters, string tableName)
        {
            lock (Connection)
            {
                try
                {
                    var cmd = new SqlCommand(procedureName, Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddRange(parameters.ToArray());

                    OpenConnection();

                    var dr = cmd.ExecuteReader();
                    var dt = new DataTable(tableName);
                    dt.Load(dr);

                    return dt;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (ConnectionAutoClose)
                        CloseConnection();
                }
            }
        }

        #endregion

        #region Delete Methods

        public void DeleteRecord(string procedureName, string parameterName, int id_key)
        {
            ExecuteNonQuery<int>(procedureName, parameterName, id_key);
        }

        #endregion

        #region ExecuteNonQuery Methods

        public void ExecuteNonQuery(string procedureName)
        {
            ExecuteNonQuery(procedureName, null);
        }

        public void ExecuteNonQuery<T>(string procedureName, string parameterName, T value)
        {
            var parameters = new List<SqlParameter>();
            parameters.Add(GetSqlParameter<T>(parameterName, value));

            ExecuteNonQuery(procedureName, parameters);
        }

        public void ExecuteNonQuery(string procedureName, IList<SqlParameter> parameters)
        {
            lock (Connection)
            {
                try
                {
                    var cmd = new SqlCommand(procedureName, Connection);
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters.ToArray());

                    OpenConnection();

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (ConnectionAutoClose)
                        CloseConnection();
                }
            }
        }

        #endregion
    }
}
