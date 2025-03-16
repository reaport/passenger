using Passenger.Infrastructure.DTO;

namespace Passenger.Services;

public class FlightRefreshServiceFake : IFlightRefreshService
{
    
    public Task<IEnumerable<FlightInfo>> GetAvailableFlights()
    {
        var flights = new List<FlightInfo>
        {
            new FlightInfo
            {
                FlightId = Guid.NewGuid().ToString(),
                EconomySeats = 50,
                VIPSeats = 30,
                RegistrationStartTime = DateTime.Now.AddSeconds(10),
                AvailableMealTypes = ["Carnivorous, Balanced"]
            },
            new FlightInfo
            {
                FlightId = Guid.NewGuid().ToString(),
                EconomySeats = 50,
                VIPSeats = 30,
                RegistrationStartTime = DateTime.Now.AddSeconds(5),
                AvailableMealTypes = ["Carnivorous, Balanced, Sunrays"]
            },
            new FlightInfo
            {
                FlightId = Guid.NewGuid().ToString(),
                EconomySeats = 50,
                VIPSeats = 30,
                RegistrationStartTime = DateTime.Now.AddSeconds(10),
                AvailableMealTypes = ["Schizo-Juice, Vegan"]
            }
        };

        return Task.FromResult<IEnumerable<FlightInfo>>(flights);
    }
}