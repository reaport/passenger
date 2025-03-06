namespace Passenger.Models;

public class PlaneStartPassengerFactory : IPassengerFactory
{
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Passenger.CreateOnPlane(mealPref, baggageWeight);
    }

}