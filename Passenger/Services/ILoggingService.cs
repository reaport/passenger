namespace Passenger.Services
{
    public interface ILoggingService
    {
        void Log<T>(string message);
        void Log<T>(LogLevel level, string message);
        List<string> GetLogs();
    }
}
