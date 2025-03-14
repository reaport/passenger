using Passenger.Infrastructure.DTO;
using Passenger.Models;
using System.Linq;

namespace Passenger.Services;

public class PassengerService : IPassengerService
{
    private List<PassengerFlightManager> _flightManagers;
    private IFlightRefreshService _flightRefreshService;
    private IServiceProvider _serviceProvider;
    public PassengerService(IFlightRefreshService flightRefreshService, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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

        var flightsToInit = availableFlights.Except(existingFlights, new FlightIdComparer());

        if(flightsToInit.Any())
        {
            foreach (var flightInfo in flightsToInit)
            {
                // This could and should be a separate factory tbh
                var strategy = _serviceProvider.GetRequiredService<IPassengerStrategy>();
                var factory = _serviceProvider.GetRequiredService<IPassengerFactory>();

                var flightManager = new PassengerFlightManager(strategy, factory, flightInfo);
                _flightManagers.Add(flightManager);
            }
        }
    }
}