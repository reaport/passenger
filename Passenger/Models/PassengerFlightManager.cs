using System.Collections.Concurrent;
using Passenger.Services;

namespace Passenger.Models;

public class PassengerFlightManager
{
    private ConcurrentBag<Passenger> _passengers;
    private IPassengerStrategy _passengerStrategy;
    public string FlightId {get; private set;}

}
