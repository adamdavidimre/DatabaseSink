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

            Test();
        }

        static void Test()
        {
            int times = 1000;
            Task task1 = CreateLoggingTask(1, times);
            Task task2 = CreateLoggingTask(2, times);
            Task task3 = CreateLoggingTask(3, times);
            Task task4 = CreateLoggingTask(4, times);
            Task task5 = CreateLoggingTask(5, times);

            Task.WaitAll(task1, task2, task3, task4, task5);
        }

        private static Task CreateLoggingTask(int id, int times)
        {
            return Task.Run(async () =>
            {
                Console.WriteLine($"Start: {id}");
                for (int i = 0; i < times; i++)
                {
                    string text = $"Task {id} - {i}";
                    Log.Information(text);
                }
                Console.WriteLine($"End: {id}");
            });
        }
    }
}
