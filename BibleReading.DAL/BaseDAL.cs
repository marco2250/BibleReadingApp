using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

using BibleReading.Common45.Root;

using BibleReading.DAL.Properties;

public class BaseDAL : IDisposable
{

    private SqlConnection _connection;
    public SqlConnection Connection
    {
        get { return _connection ?? (_connection = new SqlConnection()); }
        set
        {
            _connection = value;

            if (value != null)
            {
                _autoCloseConnection = false;
            }
        }
    }

    private bool _autoCloseConnection = true;
    public bool AutoCloseConnection
    {
        get { return _autoCloseConnection; }
        set { _autoCloseConnection = value; }
    }

    private string _connectionString = Settings.Default.ConnectionString;
    public string ConnectionString
    {
        get { return _connectionString; }
        set { _connectionString = value; }
    }

    public BaseDAL()
    {
        _connectionString = Settings.Default.ConnectionString;
        _connection = new SqlConnection(_connectionString);
	}

    public BaseDAL(string connectionString)
    {
        ConnectionString = connectionString;
    }

    #region Delete Methods
    protected void DeleteRecord(string procedureName, string parameterName, int idKey)
    {
        ExecuteNonQuery(procedureName, parameterName, idKey);
    }

    #endregion

    #region ExecuteNonQuery

    protected void ExecuteNonQuery<T>(string procedureName, string parameterName, T value)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter(parameterName, value));

        ExecuteNonQuery(procedureName, parameters);
    }

    protected void ExecuteNonQuery(string procedureName, IList<SqlParameter> parameters)
    {
        lock (Connection)
        {
            try
            {
                var cmd = new SqlCommand(procedureName, Connection);
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null)
                {
                    cmd.Parameters.AddRange(parameters.ToArray());
                }

                OpenConnection();

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (AutoCloseConnection)
                {
                    CloseConnection();
                }
            }
        }
    }


    protected void ExecuteNonQuery(string commmandString, SqlTransaction transaction, List<SqlParameter> parameters)
    {
        lock (Connection)
        {
            var cmd = new SqlCommand(commmandString, Connection);
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;

            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }

            if (parameters != null)
            {
                foreach (var p in parameters)
                {
                    cmd.Parameters.Add(p);
                }
            }

            try
            {
                OpenConnection();

                cmd.ExecuteNonQuery();
            }
            finally
            {
                if (AutoCloseConnection)
                {
                    CloseConnection();
                }
            }
        }

    }

    #endregion

    #region ExecuteScalar

    protected T ExecuteScalar<T, TP>(string cmdText, CommandType commandType, string parameterName, TP parameterValue)
    {
        return ExecuteScalar<T, TP>(cmdText, commandType, parameterName, parameterValue, null);
    }

    protected T ExecuteScalar<T, TP>(string cmdText, CommandType commandType, string parameterName, TP parameterValue, SqlTransaction transaction)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter(parameterName, parameterValue));

        return ExecuteScalar<T>(cmdText, commandType, parameters, null);
    }

    protected T ExecuteScalar<T>(string cmdText, CommandType commandType, IList<SqlParameter> parameters)
    {
        return ExecuteScalar<T>(cmdText, commandType, parameters, null);
    }

    protected T ExecuteScalar<T>(string cmdText, CommandType commandType, IList<SqlParameter> parameters, SqlTransaction transaction)
    {
        var cmd = new SqlCommand(cmdText, Connection);
        cmd.CommandType = commandType;

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters.ToArray());
        }

        return ExecuteScalar<T>(cmd, transaction);
    }

    protected T ExecuteScalar<T>(SqlCommand cmd, SqlTransaction transaction)
    {

        lock (Connection)
        {
            try
            {
                if (cmd.Connection == null)
                {
                    cmd.Connection = Connection;
                }

                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }

                OpenConnection(cmd.Connection);

                var ret = cmd.ExecuteScalar();

                return ReferenceEquals(ret, DBNull.Value) ? default(T) : ObjectUtility.CastMe<T>(ret);
            }
            finally
            {
                if (AutoCloseConnection)
                {
                    CloseConnection(cmd.Connection);
                }
            }
        }
    }

    #endregion

    #region "ExecuteQueryDataTable"

    protected DataTable GetDataTable(string procedureName)
    {
        return GetDataTable(procedureName, null, null, null);
    }

    protected DataTable GetDataTable<T>(string procedureName, string parameterName, T parameterValue)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter(parameterName, parameterValue));

        return GetDataTable(procedureName, parameters, null, null);
    }

    protected DataTable GetDataTable<T>(string procedureName, string parameterName, T parameterValue, SqlTransaction transaction)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter(parameterName, parameterValue));

        return GetDataTable(procedureName, parameters, null, transaction);
    }

    protected DataTable GetDataTable<T>(string procedureName, string parameterName, T parameterValue, string tableName, SqlTransaction transaction)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter(parameterName, parameterValue));

        return GetDataTable(procedureName, parameters, tableName, transaction);
    }

    protected DataTable GetDataTable<T>(string procedureName, string parameterName, T parameterValue, string tableName)
    {
        var parameters = new List<SqlParameter>();
        parameters.Add(GetSqlParameter(parameterName, parameterValue));

        return GetDataTable(procedureName, parameters, tableName);
    }

    protected DataTable GetDataTable(string procedureName, IList<SqlParameter> parameters)
    {
        return GetDataTable(procedureName, parameters, null, null);
    }

    protected DataTable GetDataTable(string procedureName, IList<SqlParameter> parameters, string tableName)
    {
        return GetDataTable(procedureName, parameters, tableName, null);
    }

    protected DataTable GetDataTable(string procedureName, IList<SqlParameter> parameters, SqlTransaction transaction)
    {
        return GetDataTable(procedureName, parameters, null, transaction);
    }

    protected DataTable GetDataTable(string procedureName, IList<SqlParameter> parameters, string tableName, SqlTransaction transaction)
    {
        var cmd = new SqlCommand(procedureName, Connection);
        cmd.CommandType = CommandType.StoredProcedure;

        if (parameters != null)
        {
            cmd.Parameters.AddRange(parameters.ToArray());
        }

        return GetDataTable(cmd, tableName, transaction);
    }

    protected DataTable GetDataTable(SqlCommand cmd, string tableName)
    {
        return GetDataTable(cmd, tableName, null);
    }

    protected DataTable GetDataTable(SqlCommand cmd, string tableName, SqlTransaction transaction)
    {
        lock (Connection)
        {
            try
            {
                if (string.IsNullOrEmpty(tableName))
                {
                    tableName = "MyTable";
                }

                if (cmd.Connection == null)
                {
                    cmd.Connection = Connection;
                }

                if (transaction != null)
                {
                    cmd.Transaction = transaction;
                }

                OpenConnection(cmd.Connection);

                var dr = cmd.ExecuteReader();
                var dt = new DataTable(tableName);
                dt.Load(dr);

                return dt;
            }
            finally
            {
                if (AutoCloseConnection)
                {
                    CloseConnection(cmd.Connection);
                }
            }
        }
    }

    #endregion

    public SqlParameter GetSqlParameter<T>(string name, T value)
    {
        if (!name.StartsWith("@"))
            name = "@" + name;

        return new SqlParameter(name, value);
    }

    public void OpenConnection()
    {
        OpenConnection(Connection);
    }

    public void OpenConnection(SqlConnection connection)
    {
        if (connection != null && connection.State != ConnectionState.Open)
        {
            connection.ConnectionString = ConnectionString;
            connection.Open();
        }
    }

    public void OpenConnection(bool autoCloseConnection)
    {
        AutoCloseConnection = autoCloseConnection;
        OpenConnection();
    }

    public void CloseConnection()
    {
        CloseConnection(Connection);
    }

    public void CloseConnection(SqlConnection connection)
    {
        if (connection != null && connection.State != ConnectionState.Closed)
        {
            connection.Close();
        }

        if (connection != null)
        {
            connection.Dispose();
        }
    }

    #region "IDisposable Support"
    // To detect redundant calls
    private bool _disposedValue;

    // IDisposable
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                CloseConnection();
            }
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion

}
