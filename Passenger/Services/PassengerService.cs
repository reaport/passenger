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
        private ConcurrentBag<FlightInfo> _previousFlights;
        private RefreshServiceHolder _refreshServiceHolder;
        private InteractionServiceHolder _interactionServiceHolder;
        private IServiceProvider _serviceProvider;
        private ILoggingService _loggingService;
        private readonly SemaphoreSlim _passengerExecutionSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _flightDeathSemaphore = new(1,1);

        public PassengerService(RefreshServiceHolder refreshServiceHolder,
                            InteractionServiceHolder interactionServiceHolder,
                            IServiceProvider serviceProvider, 
                            ILoggingService loggingService)
        {
            _serviceProvider = serviceProvider;
            _refreshServiceHolder = refreshServiceHolder;
            _interactionServiceHolder = interactionServiceHolder;
            _loggingService = loggingService;
            _flightManagers = new ConcurrentBag<PassengerFlightManager>();
            _previousFlights = new();

            PassengerFlightManager.OnDeadFlight += HandleFlightDeath;
        }

        public void CleanupFlights()
        {

            _flightManagers = new ConcurrentBag<PassengerFlightManager>();
  
            GC.Collect();
        }

        public async Task ExecutePassengerActions()
        {
            if (!_flightManagers.Any())
            {
                _loggingService.Log<PassengerService>(LogLevel.Information, 
                    "No initialised flights, nothing to execute");
                return;
            }

            var tasks = _flightManagers.Select(p => p.ExecutePassengerActions());

            // Create the aggregated task first
            Task whenAllTask = Task.WhenAll(tasks);

            try
            {
                await whenAllTask; // Throws the first exception encountered
            }
            catch (Exception ex)
            {
                // Check if the aggregated task has exceptions
                if (whenAllTask.Exception != null)
                {
                    // Handle all exceptions (not just the first one thrown)
                    foreach (var innerEx in whenAllTask.Exception.InnerExceptions)
                    {
                        _loggingService.Log<PassengerService>(LogLevel.Error, $"Exception from task: {innerEx.Message}");
                        
                    }
                }
                else
                {
                    // Handle the case where the exception is not from the tasks
                    // (e.g., cancellation)
                    _loggingService.Log<PassengerService>(LogLevel.Error, $"General exception {ex.Message}");
                }
            }
        }

        public async Task RefreshAndInitFlights()
        {
            var availableFlights = await _refreshServiceHolder.GetService().GetAvailableFlights();

            var existingFlights = _flightManagers
                .Select(fm => fm._flightInfo);

            var flightsToInit = availableFlights.Except(_previousFlights, new FlightIdComparer());

            if (flightsToInit.Any())
            {
                    var factory = _serviceProvider.GetRequiredKeyedService<IPassengerFactory>("Airport");
                    foreach (var flightInfo in flightsToInit)
                    {
                        _previousFlights.Add(flightInfo);
                        var flightManager = new PassengerFlightManager(factory, flightInfo, _interactionServiceHolder, _loggingService);
                        _flightManagers.Add(flightManager);
                        _loggingService.Log<PassengerService>(LogLevel.Information, 
                            $"Initialised new flight with id {flightInfo.FlightId}");
                    }
            }
        }

        private void HandleFlightDeath(PassengerFlightManager manager)
        {   
            _flightDeathSemaphore.Wait();
            try
            {
                _flightManagers = new ConcurrentBag<PassengerFlightManager>(
                    _flightManagers.Except(new[] { manager }));
            }
            finally
            {
                _flightDeathSemaphore.Release();
            }

            _loggingService.Log<PassengerService>(LogLevel.Information, 
                $"No more people left for the flight with ID {manager._flightInfo.FlightId}, cleaning up...");
        }

        public void Dispose()
        {
            _passengerExecutionSemaphore?.Dispose();
        }

        // Rest of the class remains unchanged
        public List<PassengerFlightManager> GetFlightManagers() => _flightManagers.ToList();
    }
}