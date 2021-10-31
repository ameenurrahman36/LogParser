using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LogParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            string logFilePath = builder.Build().GetSection("logFilePath").Value;

            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(c => c.AddConsole(opt => opt.LogToStandardErrorThreshold = LogLevel.Debug))
                .AddSingleton<ILogParserService, LogParserService>()
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogInformation("Starting application");

            //do the actual work here
            var logParser = serviceProvider.GetService<ILogParserService>();
            var logs = logParser.GetLogs(logFilePath);
            logs = logParser.ParseLogs(logs, ' ', "GET", "HTTP");
            var uniqueIPAddresses = logParser.UniqueIPAddresses(logs).Aggregate((i, j) => i + "," + j);
            var top3MostVisitedURLs = logParser.Top3MostVisitedURLs(logs).Aggregate((i, j) => i + "," + j);
            var top3MostActiveIPAddresses = logParser.Top3MostActiveIPAddresses(logs).Aggregate((i, j) => i + "," + j);

            logger.LogInformation("All done!");
        }
    }
}
