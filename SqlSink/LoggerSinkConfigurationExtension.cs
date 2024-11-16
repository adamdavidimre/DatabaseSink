using Serilog.Configuration;
using Serilog;
using Serilog.Sinks.PeriodicBatching;

namespace SqlSink
{
    public static class LoggerSinkConfigurationExtension
    {
        public static LoggerConfiguration DatabaseSink(
            this LoggerSinkConfiguration loggerConfiguration,
            string databaseType,
            string connectionString,
            string insertCommand,
            int batchSizeLimit,
            int periodSeconds,
            int queueLimit)
        {
            var exampleSink = new DatabaseSink(
                databaseType, 
                connectionString, 
                insertCommand);

            var batchingOptions = new PeriodicBatchingSinkOptions
            {
                BatchSizeLimit = batchSizeLimit,
                Period = TimeSpan.FromSeconds(periodSeconds),
                EagerlyEmitFirstEvent = true,
                QueueLimit = queueLimit
            };

            var batchingSink = new PeriodicBatchingSink(exampleSink, batchingOptions);

            return loggerConfiguration.Sink(batchingSink);
        }
    }
}
