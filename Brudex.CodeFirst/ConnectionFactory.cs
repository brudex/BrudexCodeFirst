using System.Collections.Generic;
using System.Data.SqlClient;

namespace Brudex.CodeFirst
{
    public class ConnectionFactory
    {
        private Connection conn;
        private string _conString;

        public ConnectionFactory(ConnectionType _connectionType, string conString)
        {
            _conString = conString;
            if (_connectionType == ConnectionType.Oracle)
            {
                conn = new OracleConnection();
            }

            if (_connectionType == ConnectionType.MySql)
            {
                conn = new SqlServerConnection();
            }

            if (_connectionType == ConnectionType.Oracle)
            {
                conn = new MySqlConnection();
            }
            if (_connectionType == ConnectionType.Postgres)
            {
                conn = new PostgresConnection();
            }
            conn._conString = _conString;
        }

        public Connection CreateConnection(string conString)
        {
            return new OracleConnection();
        }

        public void ExecuteCommand(string command)
        {
           conn.ExecuteCommand(command);
        }

        public List<ColumnMap> GetTableInformation(string tableName)
        {
            var command = SqlHelper.GetTableInformation(tableName);
            var reader=conn.ExecuteReader(command);
            var columns = new List<ColumnMap>();
            while (reader.Read())
            {
                var c = new ColumnMap();
                c.EnityTable = tableName;
                c.EnityName = tableName;
                c.ColumnName = reader["column_name"].ToString();
                c.SetFieldType(reader["data_type"].ToString());
                columns.Add(c);
            }
            reader.Close();
            return columns;
        }

        public bool IsFirstMigrations()
        {

            return false;
        }
    }

    public abstract class Connection
    {
        public string _conString;
        public void ExecuteCommand(string command)
        {
            //return "Generic";
        }

        public SqlDataReader ExecuteReader(string command)
        {
            using (var connection = new SqlConnection(_conString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(command, connection);
                var reader= cmd.ExecuteReader();
                connection.Close();
                return reader;
            }
        }

        public bool IsFirstMigrations()
        {
            return false;
            //return "Generic";
        }
    }


    public class OracleConnection : Connection
    {
        public new void ExecuteCommand(string command)
        {
        }
    }

    public class SqlServerConnection : Connection
    {
        public new void ExecuteCommand(string command)
        {
           using (var connection = new SqlConnection(_conString))
           {
               connection.Open();
               var cmd = new SqlCommand(command,connection);
               cmd.ExecuteNonQuery();
           }
        }
    }


    public class MySqlConnection : Connection
    {
        public new void ExecuteCommand(string command)
        {}
    }
    public class PostgresConnection : Connection
    {
        public new void ExecuteCommand(string command)
        {}
    }
}
