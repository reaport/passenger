namespace Passenger.Services;

public interface IPassengerStrategy
{
    public Task ExecutePassengerAction(Models.Passenger passenger);
}