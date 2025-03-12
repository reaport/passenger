using Passenger.Infrastructure.DTO;
using Passenger.Models;
using System.Linq;

namespace Passenger.Services;

public class PassengerService : IPassengerService
{
    private List<PassengerFlightManager> _flightManagers;
    private IFlightRefreshService _flightRefreshService;
    private PassengerService(IFlightRefreshService flightRefreshService)
    {
        _flightRefreshService = flightRefreshService;
        _flightManagers = new(5);
    }
    public async Task ExecutePassengerActions()
    {
        var taskList = new List<Task>(_flightManagers.Count);

        foreach (var flight in _flightManagers)
        {
            taskList.Add(flight.ExecutePassengerActions());
        }

        await Task.WhenAll(taskList);
    }

    public async Task RefreshAndInitFlights()
    {
        var availableFlights = await _flightRefreshService.GetAvailableFlights();
        
        var existingFlights = _flightManagers.AsEnumerable().Select(fm => fm._flightInfo);

        // if(availableFlights is null || existingFlights is null) throw new Exception();

        var flightsToInit = availableFlights.Except(existingFlights, new FlightIdComparer());

        if(flightsToInit.Any())
        {
            //TODO init flight manager for new flight
            
        }
        
    }
}