using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Passenger.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly List<string> _logs = new List<string>();
        private readonly ILoggerFactory _loggerFactory;

        public LoggingService(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void Log<T>(string message)
        {
            string formattedMessage = $"[{typeof(T).Name}] {message}";

            // Store log in central list
            _logs.Add(formattedMessage);

            // Get a logger specific to T and log to built-in system
            ILogger<T> logger = _loggerFactory.CreateLogger<T>();
            logger.LogInformation(formattedMessage);
        }

        public void Log<T>(LogLevel level, string message)
        {
            string formattedMessage = $"[{DateTime.UtcNow.ToString("o")}] [{level}] [{typeof(T).Name}] {message}";

            // Store log in central list
            _logs.Add(formattedMessage);

            // Get a logger for the calling type and log at the specified level
            ILogger<T> logger = _loggerFactory.CreateLogger<T>();
            logger.Log(level, formattedMessage);
        }

        public List<string> GetLogs()
        {
            return _logs;
        }
    }
}
