using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Serilog.Core;
using Serilog.Events;
using System.Data;
using System.Data.SqlClient;

namespace SqlSink
{
    public class DatabaseSink : ILogEventSink
    {
        private IDbConnection _connection;
        private readonly string _insertCommand;

        public DatabaseSink(string databaseType, string connectionString, string insertCommand)
        {
            _insertCommand = insertCommand;
            _connection = CreateConnection(databaseType, connectionString);
        }

        private IDbConnection CreateConnection(string databaseType, string connectionString)
        {
            IDbConnection connection;

            switch (databaseType.ToLower())
            {
               case "oracle":
                    connection = new OracleConnection(connectionString);
                    //Pooling
                    break;
                case "postgres":
                    connection = new NpgsqlConnection(connectionString);
                    //Pooling enabled by default https://www.npgsql.org/doc/basic-usage.html#pooling
                    break;
                default:
                    throw new ArgumentException("Unsupported database type");
            }

            return connection;
        }

        private void AddParameterToCommand(IDbCommand command, string name, object? value)
        {
            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            command.Parameters.Add(p);
        }

        public void Emit(LogEvent logEvent)
        {
            IDbCommand command = _connection.CreateCommand();
            command.CommandText = _insertCommand;

            //TODO: Implicit limitation
            AddParameterToCommand(command, "@Message", logEvent.RenderMessage());
            AddParameterToCommand(command, "@Timestamp", logEvent.Timestamp.UtcDateTime);
            AddParameterToCommand(command, "@Level", logEvent.Level.ToString());

            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            command.ExecuteNonQuery();

            //TODO:
            // *connection pooling - default supported
            // -timeout handling -> retry policy, queueing
            // -sql injection prevention - done, parameters
            // *batch + flush for performance (timebased)
            // exception handling
            // *retry policy
            // *async
            // -non blocking -> async
            // -concurrency -> async, pooling
            // -throttling, rate limit -> timebased batch
            // 



        }
    }
}
