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

    public PassengerFlightManager(IPassengerStrategy strategy, IPassengerFactory factory, FlightInfo flightInfo)
    {
        _passengers = [];
        _passengerStrategy = strategy;
        _factory = factory;
        _flightInfo = flightInfo;

        PopulatePassengerBag(_flightInfo.EconomySeats, _flightInfo.VIPSeats, flightInfo.AvailableMealTypes);
    }

    public Task PopulatePassengerBag(int plebCount, int vipCount, IEnumerable<string> mealPref)
    {
        //TODO implement proper baggage gen

        for(int i = 0; i < plebCount; i++)
        {
            _passengers.Add(_factory.Create(mealPref, 10.43f, PassengerClass.Economy));
        }

        for(int i = 0; i< vipCount; i++)
        {
            _passengers.Add(_factory.Create(mealPref, 999.0f, PassengerClass.Business));
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
}
