using System.Collections.Concurrent;
using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Models;

public class PassengerFlightManager
{
    private ConcurrentBag<Passenger> _passengers;
    private IPassengerStrategy _passengerStrategy;
    private IPassengerFactory _factory;
    public FlightInfo _flightInfo;
    private readonly SemaphoreSlim _semaphore = new(1,1);
    public static event Action<PassengerFlightManager>? OnDeadFlight;
    public PassengerFlightManager(IPassengerStrategy strategy, IPassengerFactory factory, FlightInfo flightInfo)
    {
        _passengers = [];
        _passengerStrategy = strategy;
        _factory = factory;
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
        //TODO implement proper baggage gen

        for(int i = 0; i < plebCount; i++)
        {
            _passengers.Add(_factory.Create(mealPref, 10.43f, PassengerClass.economy));
        }

        for(int i = 0; i< vipCount; i++)
        {
            _passengers.Add(_factory.Create(mealPref, 27.0f, PassengerClass.business));
        }

        return Task.CompletedTask;
    }
    public async Task<bool> ExecutePassengerActions()
    {

        await _semaphore.WaitAsync();
        var result = new List<Task<bool>>(_passengers.Count);

        foreach (var passenger in _passengers)
        {
            result.Add(_passengerStrategy.ExecutePassengerAction(passenger));
        }

        _semaphore.Release();

        try
        {
            await Task.WhenAll(result);
            return true;
        }
        catch (AggregateException e)
        {
            System.Console.WriteLine($"Aggregate exception: {e.Message}\n{e.InnerExceptions}");
        }
        catch(Exception ex)
        {   
            System.Console.WriteLine($"Unhandled exception: {ex.Message}\n{ex.StackTrace}");
        }

        return false;
    }

    ~PassengerFlightManager()
    {
        Passenger.OnPassengerDied -= HandlePassengerDeath;
    }
}
