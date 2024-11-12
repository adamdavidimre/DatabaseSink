using Microsoft.Extensions.Configuration;
using Serilog;
using SqlSink;

namespace Inheritance
{

    internal class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(AppContext.BaseDirectory)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Information("Hello, Serilog!");
            Log.Warning("Hello, Serilog!");
            Log.Error("Hello, Serilog!");
            Log.Fatal("Hello, Serilog!");
        }
    }
}
