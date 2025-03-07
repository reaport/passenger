using System.Collections.Concurrent;
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

        //TODO differentiate between VIP and plebian passengers
        PopulatePassengerBag(_flightInfo.EconomySeats, _flightInfo.VIPSeats);
    }

    public Task PopulatePassengerBag(int plebCount, int vipCount)
    {
        //TODO implement proper pref and baggage gen
        IEnumerable<string> mealPref = ["asdf", "srfd"];

        for(int i = 0; i < plebCount; i++)
        {
            _passengers.Add(_factory.Create(mealPref, 10.43f));
        }

        for(int i = 0; i< vipCount; i++)
        {
            _passengers.Add(_factory.Create(mealPref, 999.0f));
        }

        return Task.CompletedTask;
    }
    public async Task ExecutePassengerActions()
    {
        var result = new List<Task>();
        Parallel.ForEach
        (
            _passengers,
            (passenger)=> result.Add(_passengerStrategy.ExecutePassengerAction(passenger))
        );

        await Task.WhenAll(result);

    }
}
