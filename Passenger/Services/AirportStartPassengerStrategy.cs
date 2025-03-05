
namespace Passenger.Services;

public class AirportStartPassengerStrategy : IPassengerStrategy
{
    public Models.Passenger CreatePassenger(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Models.Passenger.CreateInAirportStartingPoint(mealPref, baggageWeight);
    }

    public Task ExecutePassengerAction(Models.Passenger passenger)
    {
        throw new NotImplementedException();
    }
}