using Passenger.Infrastructure.DTO;
using Passenger.Models;
using System.Collections.Concurrent;


namespace Passenger.Services
{
    public class PassengerService : IPassengerService, IDisposable
    {
        private ConcurrentBag<FlightInfo> _previousFlights;
        private ConcurrentDictionary<PassengerFlightManager, CancellationTokenSource> _managersTokens; 
        private RefreshServiceHolder _refreshServiceHolder;
        private InteractionServiceHolder _interactionServiceHolder;
        private IServiceProvider _serviceProvider;
        private ILoggingService _loggingService;

        public PassengerService(RefreshServiceHolder refreshServiceHolder,
                            InteractionServiceHolder interactionServiceHolder,
                            IServiceProvider serviceProvider, 
                            ILoggingService loggingService)
        {
            _serviceProvider = serviceProvider;
            _refreshServiceHolder = refreshServiceHolder;
            _interactionServiceHolder = interactionServiceHolder;
            _loggingService = loggingService;
            _managersTokens = new();
            _previousFlights = new();

            PassengerFlightManager.OnDeadFlight += HandleFlightDeath;
        }

        public void CleanupFlights()
        {
            foreach (var cts in _managersTokens.Values)
            {
                cts.Cancel();
                cts.Dispose();
            }
            _managersTokens.Clear();
            
            GC.Collect();
        }

        public async Task RefreshAndInitFlights()
        {
            var availableFlights = await _refreshServiceHolder.GetService().GetAvailableFlights();

            var flightsToInit = availableFlights.Except(_previousFlights, new FlightIdComparer());

            if (flightsToInit.Any())
            {
                var factory = _serviceProvider.GetRequiredKeyedService<IPassengerFactory>("Airport");
                foreach (var flightInfo in flightsToInit)
                {
                    CancellationTokenSource cts = new();
                    var flightManager = new PassengerFlightManager(factory, flightInfo, _interactionServiceHolder, _loggingService);

                    // Add to managers dictionary
                    if(!_managersTokens.TryAdd(flightManager, cts)) 
                    {
                        _loggingService.Log<PassengerService>(LogLevel.Error, $"Failed to add new flight with id {flightInfo.FlightId}");
                        return;
                    }
                    _previousFlights.Add(flightInfo);

                    _loggingService.Log<PassengerService>(LogLevel.Information, 
                        $"Initialised new flight with id {flightInfo.FlightId}");

                    // Start flight handling task
                    _ = HandleFlight(flightManager, TimeSpan.FromSeconds(5), cts.Token);

                }

                return;
            }

            _loggingService.Log<PassengerService>(LogLevel.Information, $"No new flights found to initialise");
        }

        private async Task HandleFlight(PassengerFlightManager flightManager, TimeSpan actionInterval, CancellationToken cancellationToken)
        {
            PeriodicTimer timer = new(actionInterval);

            while(await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
            {
                var task = flightManager.ExecutePassengerActions();
                
                try
                {
                    await task;
                }
                catch (Exception ex)
                {
                    if (task.Exception != null)
                    {
                        foreach (var innerEx in task.Exception.InnerExceptions)
                        {
                            _loggingService.Log<PassengerService>(LogLevel.Error, 
                                $"Exception from task: {innerEx.Message}\n{innerEx.StackTrace}");
                        }
                    }
                    else
                    {
                        _loggingService.Log<PassengerService>(LogLevel.Error, 
                            $"General exception: {ex.Message}");
                    }
                }
            }
        }

        private void HandleFlightDeath(PassengerFlightManager manager)
        {   
            // Remove manager and cancel its token
            if (_managersTokens.TryRemove(manager, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }

            _loggingService.Log<PassengerService>(LogLevel.Information, 
                $"No more people left for flight {manager._flightInfo.FlightId}, cleaning up...");
        }

        // Return active flight managers
        public List<PassengerFlightManager> GetFlightManagers() => _managersTokens.Keys.ToList();

        public void Dispose()
        {
            CleanupFlights();
            PassengerFlightManager.OnDeadFlight -= HandleFlightDeath;
        }
    }
}