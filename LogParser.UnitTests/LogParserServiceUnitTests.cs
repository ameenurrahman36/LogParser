using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LogParser.UnitTests
{
    public class LogParserServiceUnitTests
    {
        private LogParserService _logParserService;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<LogParserService>>();
            mockLogger.Setup(
                m => m.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()));

            var mockLoggerFactory = new Mock<ILoggerFactory>();
            mockLoggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(() => mockLogger.Object);
            _logParserService = new LogParserService(mockLoggerFactory.Object);
        }

        [TestCase("C:\\temp\\LogParser\\LogParser\\programming-task-example-data.log")]
        public void Valid_LogFilePath_ReturnsRecords(string value)
        {
            var result = _logParserService.GetLogs(value);

            Assert.IsFalse(result.Count == 0);
        }

        [TestCase("C:\\temp\\programming-task-example-data.log")]
        public void InValid_LogFilePath_ReturnsZeroRecords(string value)
        {
            var result = _logParserService.GetLogs(value);

            Assert.IsTrue(result.Count == 0);
        }

        [TestCase("C:\\temp\\LogParser\\LogParser\\programming-task-example-data.log", 23)]
        public void LogFile_WithValid_Parameters(string value, int count)
        {
            var result = _logParserService.GetLogs(value);

            var logs = _logParserService.ParseLogs(result, ' ', "GET", "HTTP");

            Assert.IsTrue(logs.Count == count);
        }

        [TestCase("C:\\temp\\LogParser\\LogParser\\programming-task-example-data.log")]
        public void LogFile_WithInValid_Parameters(string value)
        {
            var result = _logParserService.GetLogs(value);

            var logs = _logParserService.ParseLogs(result, ' ', "POST", "HTTP");

            Assert.IsTrue(logs.Any(u => string.IsNullOrEmpty(u.URL)));
        }

        [TestCase("C:\\temp\\LogParser\\LogParser\\programming-task-example-data.log")]
        public void Valid_UniqueIPAddresses(string value)
        {
            var result = _logParserService.GetLogs(value);

            var logs = _logParserService.ParseLogs(result, ' ', "GET", "HTTP");

            var actual = _logParserService.UniqueIPAddresses(logs);

            IEnumerable<string> expected = new string[] 
            {"177.71.128.21","168.41.191.40","168.41.191.41","168.41.191.9","168.41.191.34","50.112.00.28","50.112.00.11","72.44.32.11","72.44.32.10","168.41.191.43","79.125.00.21"};

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase("C:\\temp\\LogParser\\LogParser\\programming-task-example-data.log")]
        public void Valid_Top3MostVisitedURLs(string value)
        {
            var result = _logParserService.GetLogs(value);

            var logs = _logParserService.ParseLogs(result, ' ', "GET", "HTTP");

            var actual = _logParserService.Top3MostVisitedURLs(logs);

            IEnumerable<string> expected = new string[]
            {"/docs/manage-websites/","/intranet-analytics/","http://example.net/faq/"};

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestCase("C:\\temp\\LogParser\\LogParser\\programming-task-example-data.log")]
        public void Valid_Top3MostActiveIPAddresses(string value)
        {
            var result = _logParserService.GetLogs(value);

            var logs = _logParserService.ParseLogs(result, ' ', "GET", "HTTP");

            var actual = _logParserService.Top3MostActiveIPAddresses(logs);

            IEnumerable<string> expected = new string[]
            {"168.41.191.40","177.71.128.21","50.112.00.11"};

            CollectionAssert.AreEqual(expected, actual);
        }
    }
}