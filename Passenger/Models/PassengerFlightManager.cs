using System.Collections.Concurrent;
using Passenger.Services;

namespace Passenger.Models;

public class PassengerFlightManager
{
    private ConcurrentBag<Passenger> _passengers;
    public string FlightId {get; private set;}
    private IPassengerStrategy _passengerStrategy;
    
    
}
