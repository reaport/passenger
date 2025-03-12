namespace Passenger.Services;

public interface IPassengerStrategy
{
    public Guid FlightId {get;set;}
    public Task ExecutePassengerAction(Models.Passenger passenger);
}