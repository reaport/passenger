namespace Passenger.Infrastructure.DTO;

public class FlightInfo
{
    public Guid FlightId {get;set;}
    public int EconomySeats {get;set;}
    public int VIPSeats {get;set;}
    public required IEnumerable<string> AvailableMealTypes {get;set;}
    //public DateTime RegistratationStartTime {get;set;}
    //public DateTime RegistrationEndTime {get;set;}
}

public class FlightIdComparer : IEqualityComparer<FlightInfo>
{
    public bool Equals(FlightInfo? x, FlightInfo? y)
    {
        if (x is null || y is null)
            return false;
        
        return x.FlightId == y.FlightId;
    }
    public int GetHashCode(FlightInfo obj)
    {
        return obj.FlightId.GetHashCode();
    }
}