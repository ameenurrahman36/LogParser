using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{
    public class LogParserService : ILogParserService
    {
        private readonly ILogger<LogParserService> _logger;

        public LogParserService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<LogParserService>();
        }

        public IList<Log> ParseLogs(IList<Log> logs, char searchValue1, string searchValue2, string searchValue3)
        {
            foreach (Log log in logs)
            {
                string[] parse = log.LogLine.Split(new[] { searchValue1 }, 2);

                if (parse.Length > 1)
                {
                    if (!string.IsNullOrEmpty(parse[0]))
                        log.IPAddress = parse[0];

                    if (!string.IsNullOrEmpty(parse[1]))
                    {
                        if (parse[1].IndexOf(searchValue2) >= 0)
                        {
                            int pFrom = parse[1].IndexOf(searchValue2) + searchValue2.Length;

                            if (parse[1].LastIndexOf(searchValue3) >= 0)
                            {
                                int pTo = parse[1].LastIndexOf(searchValue3);

                                string url = parse[1].Substring(pFrom, pTo - pFrom);
                                if (!string.IsNullOrEmpty(url))
                                    log.URL = url;
                            }
                        }
                    }
                }
            }

            return logs;
        }

        public IList<Log> GetLogs(string logFilePath)
        {
            _logger.LogInformation($"Doing the thing {logFilePath}");

            IList<Log> logs = new List<Log>();

            if (File.Exists(logFilePath))
            {
                // Read the file and display it line by line.  
                foreach (string line in System.IO.File.ReadLines(logFilePath))
                {
                    logs.Add(new Log { LogLine = line });
                }
            }

            return logs;
        }

        public IEnumerable<string> UniqueIPAddresses(IList<Log> logs)
        {
            //The number of unique IP addresses
            return logs.DistinctBy(log => new { log.IPAddress }).Select(l => l.IPAddress);
        }

        public IEnumerable<string> Top3MostVisitedURLs(IList<Log> logs)
        {
            //The top 3 most visited URLs
            return (from id in logs.Select(l => l.URL)
                    group id by id into g
                    orderby g.Count() descending
                    select new { url = g.Key }).Take(3).Select(u => u.url.Trim());
        }

        public IEnumerable<string> Top3MostActiveIPAddresses(IList<Log> logs)
        {
            //The top 3 most active IP addresses
            return (from id in logs.Select(l => l.IPAddress)
                    group id by id into g
                    orderby g.Count() descending
                    select new { ipAddress = g.Key }).Take(3).Select(i => i.ipAddress);
        }
    }
}
