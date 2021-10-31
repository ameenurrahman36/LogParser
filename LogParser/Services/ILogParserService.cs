using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogParser
{
    public interface ILogParserService
    {
        IList<Log> GetLogs(string logFilePath);
        IList<Log> ParseLogs(IList<Log> logs, char searchValue1, string searchValue2, string searchValue3);
        IEnumerable<string> UniqueIPAddresses(IList<Log> logs);
        IEnumerable<string> Top3MostVisitedURLs(IList<Log> logs);
        IEnumerable<string> Top3MostActiveIPAddresses(IList<Log> logs);
    }
}
