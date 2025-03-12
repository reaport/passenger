using Passenger.Infrastructure.DTO;

namespace Passenger.Services;

public interface IFlightRefreshService
{
    Task<IEnumerable<FlightInfo>> GetAvailableFlights();
}