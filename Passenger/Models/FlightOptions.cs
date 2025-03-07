namespace Passenger.Models;

public class FlightInfo
{
    public Guid FlightId {get;set;}
    public int EconomySeats {get;set;}
    public int VIPSeats {get;set;}
    public IEnumerable<string> AvailableMealOptions {get;set;}
    public DateTime RegistratationStartTime {get;set;}
    public DateTime RegistrationEndTime {get;set;}
}   