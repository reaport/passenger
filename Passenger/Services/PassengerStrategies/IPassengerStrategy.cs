namespace Passenger.Services;

public interface IPassengerStrategy
{
    //public string FlightId {get;set;}
    public Task<bool> ExecutePassengerAction(Models.Passenger passenger);
}