
using Passenger.Models;

namespace Passenger.Services;

public class PassengerService : IPassengerService
{
    private List<PassengerFlightManager> _flightManagers;
    private PassengerService()
    {
        _flightManagers = new(5);
    }
    public async Task ExecutePassengerActions()
    {
        //throw new NotImplementedException();
    }

    public async Task RefreshAvailableFlights()
    {
        //throw new NotImplementedException();
    }
}