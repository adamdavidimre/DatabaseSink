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

            //TestNoStop5ThreadPer10000();
            Test1msStop10ThreadPer10000();

            Thread.Sleep(100000);
        }

        static void TestNoStop5ThreadPer10000()
        {
            int times = 10000;
            Task task1 = CreateLoggingTask(1, times, 0);
            Task task2 = CreateLoggingTask(2, times, 0);
            Task task3 = CreateLoggingTask(3, times, 0);
            Task task4 = CreateLoggingTask(4, times, 0);
            Task task5 = CreateLoggingTask(5, times, 0);

            Task.WaitAll(task1, task2, task3, task4, task5);
        }

        static void Test1msStop10ThreadPer10000()
        {
            int times = 10000;
            Task task1 = CreateLoggingTask(1, times, 1);
            Task task2 = CreateLoggingTask(2, times, 1);
            Task task3 = CreateLoggingTask(3, times, 1);
            Task task4 = CreateLoggingTask(4, times, 1);
            Task task5 = CreateLoggingTask(5, times, 1);
            Task task6 = CreateLoggingTask(6, times, 1);
            Task task7 = CreateLoggingTask(7, times, 1);
            Task task8 = CreateLoggingTask(8, times, 1);
            Task task9 = CreateLoggingTask(9, times, 1);
            Task task10 = CreateLoggingTask(10, times, 1);

            Task.WaitAll(task1, task2, task3, task4, task5);
        }

        private static Task CreateLoggingTask(int id, int times, int waitMs)
        {
            return Task.Run(async () =>
            {
                Thread.Sleep(waitMs);
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
