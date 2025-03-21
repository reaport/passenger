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
                FlightId = "HONK001",
                EconomySeats = 5,
                VIPSeats = 2,
                RegistrationStartTime = DateTime.Now.AddSeconds(5),
                BoardingStart = DateTime.Now.AddSeconds(15),
                DepartureTime = DateTime.Now.AddSeconds(25),
                AvailableMealTypes = ["Carnivorous, Balanced"]
            },
            new FlightInfo
            {
                FlightId = "HIPPIE002",
                EconomySeats = 10,
                VIPSeats = 5,
                RegistrationStartTime = DateTime.Now.AddSeconds(5),
                BoardingStart = DateTime.Now.AddSeconds(15),
                DepartureTime = DateTime.Now.AddSeconds(25),
                AvailableMealTypes = ["Carnivorous, Balanced, Sunrays"]
            },
            new FlightInfo
            {
                FlightId = "ACIDBATH003",
                EconomySeats = 15,
                VIPSeats = 3,
                RegistrationStartTime = DateTime.Now.AddSeconds(5),
                BoardingStart = DateTime.Now.AddSeconds(15),
                DepartureTime = DateTime.Now.AddSeconds(25),
                AvailableMealTypes = ["Schizo-Juice, Vegan"]
            }
        };

        return Task.FromResult<IEnumerable<FlightInfo>>(flights);
    }
}