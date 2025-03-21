using System.Collections.Concurrent;
using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Models;

public class PassengerFlightManager
{
    private ConcurrentBag<Passenger> _passengers;
    private IPassengerFactory _factory;
    private InteractionServiceHolder _interactionServiceHolder;
    public FlightInfo _flightInfo;
    private readonly ILoggingService _logger;
    private readonly SemaphoreSlim _semaphore = new(1,1);
    public static event Action<PassengerFlightManager>? OnDeadFlight;
    public PassengerFlightManager(IPassengerFactory factory, FlightInfo flightInfo, InteractionServiceHolder interactionServiceHolder, ILoggingService loggingService)
    {
        _passengers = [];
        _factory = factory;
        _interactionServiceHolder = interactionServiceHolder;
        _flightInfo = flightInfo;
        _logger = loggingService;

        PopulatePassengerBag(_flightInfo.EconomySeats, _flightInfo.VIPSeats, flightInfo.AvailableMealTypes);
        
        Passenger.OnPassengerDied += HandlePassengerDeath;
    }

    private void HandlePassengerDeath(Passenger passenger)
    {
        _passengers = new(_passengers.Where(p => p != passenger));
        if(_passengers.IsEmpty) OnDeadFlight?.Invoke(this);
    }
    public int GetPassengerCount()
    {
        return _passengers.Count();
    }
    public Task PopulatePassengerBag(int plebCount, int vipCount, IEnumerable<string> mealPref)
    {
        for(int i = 0; i < plebCount; i++)
        {
            _passengers.Add(_factory.Create(_interactionServiceHolder, 5.0f, "Economy", _flightInfo));
        }

        for(int i = 0; i< vipCount; i++)
        {
            _passengers.Add(_factory.Create(_interactionServiceHolder, 7.0f, "Business", _flightInfo));
        }

        return Task.CompletedTask;
    }
    public async Task ExecutePassengerActions()
    {
        var tasks = _passengers.Select( p => p.ExecuteNextStep());

        var whenAllTask = Task.WhenAll(tasks);
        
        try
        {
            await whenAllTask;
        }
        catch (Exception ex)
        {
            // Check if the aggregated task has exceptions
            if (whenAllTask.Exception != null)
            {
                // Handle all exceptions (not just the first one thrown)
                foreach (var innerEx in whenAllTask.Exception.InnerExceptions)
                {
                    _logger.Log<PassengerService>(LogLevel.Error, $"Exception from task: {innerEx.Message}");
                }
            }
            else
            {
                // Handle the case where the exception is not from the tasks
                // (e.g., cancellation)
                _logger.Log<PassengerService>(LogLevel.Error, $"General exception {ex.Message}");
            }
        }
    }

    ~PassengerFlightManager()
    {
        Passenger.OnPassengerDied -= HandlePassengerDeath;
    }
}
