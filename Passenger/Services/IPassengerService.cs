namespace Passenger.Services;

public interface IPassengerService
{
    public Task RefreshAndInitFlights();
    public Task ExecutePassengerActions();
}