namespace Passenger.Services;

public interface IDriverService: IHostedService, IDisposable
{
    Task CleanUpFlights();
    Task Pause();
    Task Resume();
}