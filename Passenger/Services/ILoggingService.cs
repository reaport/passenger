using System.Collections.Generic;

namespace Passenger.Services
{
    public interface ILoggingService
    {
        void Log(string message);
        List<string> GetLogs();
    }
}