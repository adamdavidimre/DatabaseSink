using Npgsql;
using Oracle.ManagedDataAccess.Client;
using Serilog.Events;
using System.Data;

namespace SqlSink
{
    public class DatabaseSink : Serilog.Sinks.PeriodicBatching.IBatchedLogEventSink
    {
        private readonly string _insertCommand;
        private readonly string _databaseType;
        private readonly string _connectionString;

        public DatabaseSink(string databaseType, string connectionString, string insertCommand)
        {
            _insertCommand = insertCommand;
            _connectionString = connectionString;
            _databaseType = databaseType;
        }

        private IDbConnection CreateConnection(string databaseType, string connectionString)
        {
            IDbConnection connection;

            switch (databaseType.ToLower())
            {
               case "oracle":
                    connection = new OracleConnection(connectionString);
                    break;
                case "postgres":
                    connection = new NpgsqlConnection(connectionString);
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
        private void PopulateCommand(IDbCommand command, List<LogEvent> logEvents)
        {
            int counter = 0;

            foreach (var entry in logEvents)
            {
                AddParameterToCommand(command, $"@Message{counter}", entry.RenderMessage());
                AddParameterToCommand(command, $"@Timestamp{counter}", entry.Timestamp.UtcDateTime);
                AddParameterToCommand(command, $"@Level{counter}", entry.Level.ToString());

                command.CommandText +=
                $"INSERT INTO public.logs (\"Timestamp\", \"Level\", \"Message\") VALUES (@Timestamp{counter}, @Level{counter}, @Message{counter});";
                
                counter++;
            }
        }

        public async Task EmitBatchAsync(IEnumerable<LogEvent> batch)
        {
            IDbConnection connection = default;

            try
            {
                connection = CreateConnection(_databaseType, _connectionString);

                IDbCommand command = connection.CreateCommand();

                PopulateCommand(command, batch.ToList());

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                connection.Close();
            }

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
        public Task OnEmptyBatchAsync()
        {
            Console.WriteLine("Empty batch.");

            return Task.CompletedTask;
        }
    }
}
