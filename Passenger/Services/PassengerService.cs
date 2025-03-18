using Passenger.Infrastructure.DTO;
using Passenger.Models;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Passenger.Services
{
    public class PassengerService : IPassengerService, IDisposable
    {
        private ConcurrentBag<PassengerFlightManager> _flightManagers;
        private RefreshServiceHolder _refreshServiceHolder;
        private IServiceProvider _serviceProvider;
        private ILoggingService _loggingService;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public PassengerService(RefreshServiceHolder refreshServiceHolder, 
                              IServiceProvider serviceProvider, 
                              ILoggingService loggingService)
        {
            _serviceProvider = serviceProvider;
            _refreshServiceHolder = refreshServiceHolder;
            _loggingService = loggingService;
            _flightManagers = new ConcurrentBag<PassengerFlightManager>();

            PassengerFlightManager.OnDeadFlight += HandleFlightDeath;
        }

        public void CleanupFlights()
        {
            _semaphore.Wait();
            try
            {
                _flightManagers = new ConcurrentBag<PassengerFlightManager>();
            }
            finally
            {
                _semaphore.Release();
            }
            GC.Collect();
        }

        public async Task ExecutePassengerActions()
        {
            if (!_flightManagers.Any())
            {
                _loggingService.Log<PassengerService>(LogLevel.Information, 
                    "No initialised flights present");
                return;
            }

            await _semaphore.WaitAsync();
            var result = new List<Task>();
            try
            {
                _loggingService.Log<PassengerService>(LogLevel.Debug, 
                    "Started creating flight manager execute tasks");
                
                var flights = _flightManagers.ToList();
                foreach (var flight in flights)
                {
                    result.Add(flight.ExecutePassengerActions());
                }
            }
            finally
            {
                _semaphore.Release();
            }

            await Task.WhenAll(result);
            return;
        }

        public async Task RefreshAndInitFlights()
        {
            var availableFlights = await _refreshServiceHolder.GetService().GetAvailableFlights();

            var existingFlights = _flightManagers.AsEnumerable()
                .Select(fm => fm._flightInfo);

            var flightsToInit = availableFlights.Except(existingFlights, new FlightIdComparer());

            if (flightsToInit.Any())
            {
                await _semaphore.WaitAsync();
                try
                {
                    foreach (var flightInfo in flightsToInit)
                    {
                        var strategy = new AirportStartPassengerStrategy(flightInfo, _loggingService);
                        var factory = _serviceProvider.GetRequiredKeyedService<IPassengerFactory>("Airport");
                        var flightManager = new PassengerFlightManager(strategy, factory, flightInfo);
                        _flightManagers.Add(flightManager);
                        _loggingService.Log<PassengerService>(LogLevel.Information, 
                            $"Initialised new flight with id {flightInfo.FlightId}");
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }

        private void HandleFlightDeath(PassengerFlightManager manager)
        {   
            _semaphore.Wait();
            try
            {
                _flightManagers = new ConcurrentBag<PassengerFlightManager>(
                    _flightManagers.Except(new[] { manager }));
            }
            finally
            {
                _semaphore.Release();
            }

            _loggingService.Log<PassengerService>(LogLevel.Information, 
                $"No more people left for the flight with ID {manager._flightInfo.FlightId}, cleaning up...");
        }

        public void Dispose()
        {
            _semaphore?.Dispose();
        }

        // Rest of the class remains unchanged
        public List<PassengerFlightManager> GetFlightManagers() => _flightManagers.ToList();
    }
}