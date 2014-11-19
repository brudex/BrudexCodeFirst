using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Brudex.CodeFirst
{
    public class ConnectionFactory
    {
        private IConnection conn;
        private string _conString;

        public ConnectionFactory(ConnectionType _connectionType, string conString)
        {
            //TODO : When done for first release remove all connection types and leave only SqlServer
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
            else
            {
                conn = new SqlServerConnection();
            }

            conn._conString = _conString;
        }



        public void ExecuteCommand(string command)
        {
            conn.ExecuteCommand(command);
        }

        public List<ColumnMap> GetTableInformation(string tableName)
        {
            var command = SqlHelper.GetTableInformation(tableName);
             
            var columns = new List<ColumnMap>();
            using (var reader = conn.ExecuteReader(command))
            {
                while (reader.Read())
                {
                    var c = new ColumnMap();
                    c.EnityTable = tableName;
                    c.EnityName = tableName;
                    c.ColumnName = reader["column_name"].ToString();
                    c.SetFieldType(reader["data_type"].ToString());
                    columns.Add(c);
                }
            }
            
            return columns;
        }

        public bool IsFirstMigrations(List<string> tableNames)
        {
            var sql = SqlHelper.GetTopInformationSchema();
            var selectedTableNames = new List<string>();
            using (var reader = conn.ExecuteReader(sql))
            {
                while (reader.Read())
                {
                    var c = reader["TABLE_NAME"].ToString();
                    selectedTableNames.Add(c);
                }
            }

            bool isFirst = true;
            if (selectedTableNames.Count > 0)
            {
                foreach (var tableName in tableNames)
                {
                    if (selectedTableNames.Contains(tableName))
                    {
                        
                        isFirst= false;
                        break;
                    }
                }
            }
             
            return isFirst ;
        }
    }

    internal interface IConnection
    {
        string _conString { get; set; }
        void ExecuteCommand(string command);
        IDataReader ExecuteReader(string command);

    }


    internal class OracleConnection : IConnection
    {
        public string _conString { get; set; }

        public void ExecuteCommand(string command)
        {
            using (var connection = new System.Data.OracleClient.OracleConnection(_conString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }
        }

        public IDataReader ExecuteReader(string command)
        {
            using (var connection = new System.Data.OracleClient.OracleConnection(_conString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = command;
                var reader = cmd.ExecuteReader();
                connection.Close();
                return reader;
            }
        }

    }

    internal class SqlServerConnection : IConnection
    {
        public string _conString { get; set; }

        public void ExecuteCommand(string command)
        {
            using (var connection = new SqlConnection(_conString))
            {
                connection.Open();
                var cmd = new SqlCommand(command, connection);
                cmd.ExecuteNonQuery();
            }
        }

        public IDataReader ExecuteReader(string command)
        {
            var connection = new SqlConnection(_conString);
            
                connection.Open();
                SqlCommand cmd = new SqlCommand(command, connection);
                var reader = cmd.ExecuteReader();
                return reader;
            
        }


    }


    internal class MySqlConnection : IConnection
    {
        public string _conString { get; set; }
        public void ExecuteCommand(string command)
        {

            using (var conn = new MySql.Data.MySqlClient.MySqlConnection(_conString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }

        }



        public IDataReader ExecuteReader(string command)
        {
            using (var connection = new SqlConnection(_conString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(command, connection);
                var reader = cmd.ExecuteReader();
                connection.Close();
                return reader;
            }
        }
    }
    internal class PostgresConnection : IConnection
    {
        public string _conString { get; set; }
        public void ExecuteCommand(string command)
        {

            using (var conn = new Npgsql.NpgsqlConnection(_conString))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }

        }



        public IDataReader ExecuteReader(string command)
        {
            using (var connection = new Npgsql.NpgsqlConnection(_conString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = command;
                var reader = cmd.ExecuteReader();
                connection.Close();
                return reader;
            }
        }
    }
}
