using Serilog.Configuration;
using Serilog;
using Serilog.Sinks.PeriodicBatching;

namespace SqlSink
{
    public static class DatabaseSinkExtensions
    {
        public static LoggerConfiguration DatabaseSink(
            this LoggerSinkConfiguration loggerConfiguration,
            string databaseType,
            string connectionString,
            string insertCommand)
        {
            var exampleSink = new DatabaseSink(databaseType, connectionString, insertCommand);

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = 100,
                Period = TimeSpan.FromSeconds(2),
                EagerlyEmitFirstEvent = true,
                QueueLimit = 10000
            };

            var batchingSink = new PeriodicBatchingSink(exampleSink, batchingOptions);

            return loggerConfiguration.Sink(batchingSink);

        }
    }
}
