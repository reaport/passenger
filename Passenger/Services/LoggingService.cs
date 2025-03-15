using System.Collections.Generic;

namespace Passenger.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly List<string> _logs = new List<string>();
        private ILogger<LoggingService> _logger;

        public LoggingService(ILogger<LoggingService> logger)
        {
            _logger = logger;
        }

        public void Log(string message)
        {
            _logs.Add(message);
            _logger.LogInformation(message);
        }

        public List<string> GetLogs()
        {
            return _logs;
        }
    }
}