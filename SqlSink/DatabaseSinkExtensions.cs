using Serilog.Configuration;
using Serilog;

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
            return loggerConfiguration.Sink(new DatabaseSink(databaseType, connectionString, insertCommand));
        }
    }
}
