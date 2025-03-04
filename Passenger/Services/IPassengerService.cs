namespace Passenger.Services;

public interface IPassengerService
{
    public Task RefreshAvailableFlights();
    public Task ExecutePassengerActions();
}