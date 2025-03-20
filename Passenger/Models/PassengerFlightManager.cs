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
    private readonly SemaphoreSlim _semaphore = new(1,1);
    public static event Action<PassengerFlightManager>? OnDeadFlight;
    public PassengerFlightManager(IPassengerFactory factory, FlightInfo flightInfo, InteractionServiceHolder interactionServiceHolder)
    {
        _passengers = [];
        _factory = factory;
        _interactionServiceHolder = interactionServiceHolder;
        _flightInfo = flightInfo;

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
        //await _semaphore.WaitAsync();
        var result = new List<Task<bool>>(_passengers.Count);

        foreach (var passenger in _passengers)
        {
            result.Add(passenger.ExecuteNextStep());
        }

        //_semaphore.Release();
        try
        {
            var task = await Task.WhenAll(result);
            return;
        }
        catch (AggregateException e)
        {
            System.Console.WriteLine($"Aggregate exception: {e.Message}\n{e.InnerExceptions}");
        }
        catch(Exception ex)
        {   
            System.Console.WriteLine($"Unhandled exception: {ex.Message}\n{ex.StackTrace}");
        }
    }

    ~PassengerFlightManager()
    {
        Passenger.OnPassengerDied -= HandlePassengerDeath;
    }
}
