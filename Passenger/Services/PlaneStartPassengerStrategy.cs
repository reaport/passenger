

namespace Passenger.Services;

public class PlaneStartPassengerStrategy : IPassengerStrategy
{
    // THIS WONT WORK DO SOMETHING ELSE IDIOT
    private Dictionary<Models.PassengerStatus, Func<Task>> _passengerActionDict; 
    public Models.Passenger CreatePassenger(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Models.Passenger.CreateOnPlane(mealPref, baggageWeight);
    }

    public Task ExecutePassengerAction(Models.Passenger passenger)
    {
        throw new NotImplementedException();
    }
}