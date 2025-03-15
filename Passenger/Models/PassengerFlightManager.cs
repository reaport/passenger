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
        _passengers = new ConcurrentBag<Passenger>(_passengers.Where(p => p != passenger));
        if(!_passengers.Any()) OnDeadFlight?.Invoke(this);
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
    public async Task ExecutePassengerActions()
    {
        var result = new List<Task>(_passengers.Count);
        Parallel.ForEach
        (
            _passengers,
            (passenger)=> result.Add(_passengerStrategy.ExecutePassengerAction(passenger))
        );

        await Task.WhenAll(result);
    }

    ~PassengerFlightManager()
    {
        Passenger.OnPassengerDied -= HandlePassengerDeath;
    }
}
