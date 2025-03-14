namespace Passenger.Services;

public interface IDriverService: IHostedService, IDisposable
{
    void Pause();
    void Resume();
}