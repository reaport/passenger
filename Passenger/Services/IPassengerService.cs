using Passenger.Models;

namespace Passenger.Services;

public interface IPassengerService
{
    public List<PassengerFlightManager> GetFlightManagers();
    public Task RefreshAndInitFlights();
    //public Task ExecutePassengerActions();
    public void CleanupFlights();
}