using Passenger.Models;

namespace Passenger.Services;

public class FlightRefreshService : IFlightRefreshService
{
    public Task<IEnumerable<FlightInfo>> GetAvailableFlights()
    {
        throw new NotImplementedException();
    }
}