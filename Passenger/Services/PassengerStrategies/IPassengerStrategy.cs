namespace Passenger.Services;

public interface IPassengerStrategy
{
    //public string FlightId {get;set;}
    public Task ExecutePassengerAction(Models.Passenger passenger);
}