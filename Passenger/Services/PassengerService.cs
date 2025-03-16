using Passenger.Infrastructure.DTO;
using Passenger.Models;
using System.Linq;

namespace Passenger.Services
{
    public class PassengerService : IPassengerService
    {
        private List<PassengerFlightManager> _flightManagers;
        private RefreshServiceHolder _refreshServiceHolder;
        private IServiceProvider _serviceProvider;
        private ILoggingService _loggingService;

        public PassengerService(RefreshServiceHolder refreshServiceHolder, IServiceProvider serviceProvider, ILoggingService loggingService)
        {
            _serviceProvider = serviceProvider;
            _refreshServiceHolder = refreshServiceHolder;
            _loggingService = loggingService;
            _flightManagers = new(5);

            PassengerFlightManager.OnDeadFlight += HandleFlightDeath;
        }

        public async Task ExecutePassengerActions()
        {
            if (!_flightManagers.Any())
            {
                _loggingService.Log<PassengerService>(LogLevel.Information, "No flights present, no actions to execute");
                await Task.Delay(10000);
                return;
            }

            var taskList = new List<Task>(_flightManagers.Count);

            foreach (var flight in _flightManagers)
            {
                taskList.Add(flight.ExecutePassengerActions());
            }

            await Task.WhenAll(taskList);
        }

        public List<PassengerFlightManager> GetFlightManagers()
        {
            return _flightManagers;
        }

        public async Task RefreshAndInitFlights()
        {
            var availableFlights = await _refreshServiceHolder.GetService().GetAvailableFlights();
            _loggingService.Log<PassengerService>(LogLevel.Information,"Refreshed flights");

            var existingFlights = _flightManagers.AsEnumerable().Select(fm => fm._flightInfo);

            var flightsToInit = availableFlights.Except(existingFlights, new FlightIdComparer());

            if (flightsToInit.Any())
            {
                foreach (var flightInfo in flightsToInit)
                {
                    // This could and should be a separate factory tbh
                    var strategy = new AirportStartPassengerStrategy(flightInfo);
                    var factory = _serviceProvider.GetRequiredKeyedService<IPassengerFactory>("Airport");

                    var flightManager = new PassengerFlightManager(strategy, factory, flightInfo);
                    _flightManagers.Add(flightManager);
                    _loggingService.Log<PassengerService>(LogLevel.Information, $"Initialised new flight with id {flightInfo.FlightId}");
                }
            }
        }

        private void HandleFlightDeath(PassengerFlightManager manager)
        {
            _flightManagers.Remove(manager);
            _loggingService.Log<PassengerService>(LogLevel.Information, $"No more people left for the flight with ID {manager._flightInfo.FlightId}, cleaning up...");
        }
    }
}