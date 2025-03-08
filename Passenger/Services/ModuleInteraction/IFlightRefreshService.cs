using Passenger.Models;

namespace Passenger.Services;

public interface IFlightRefreshService
{
    Task<IEnumerable<FlightInfo>> GetAvailableFlights();
}