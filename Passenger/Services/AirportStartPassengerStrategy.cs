
using Passenger.Models;

namespace Passenger.Services;

public class AirportStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task>> _passengerSteps;
    public AirportStartPassengerFactory _factory;
    public Models.Passenger CreatePassenger(IEnumerable<string> mealPref, float baggageWeight)
    {
        return _factory.Create(mealPref, baggageWeight);
    }

    public async Task ExecutePassengerAction(Models.Passenger passenger)
    {
        if(_passengerSteps.Count > 0)
        {
            var currentStep = _passengerSteps.Dequeue();
            await currentStep(passenger);
        }
    }
}