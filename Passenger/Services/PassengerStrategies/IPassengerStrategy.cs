namespace Passenger.Services;

public interface IPassengerStrategy
{
    //public string FlightId {get;set;}
    public void SetFlightId(string flightId);
    public string GetFlightId();
    public Task ExecutePassengerAction(Models.Passenger passenger);
}