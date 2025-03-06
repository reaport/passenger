namespace Passenger.Models;

public class AirportStartPassengerFactory : IPassengerFactory
{
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Passenger.CreateInAirportStartingPoint(mealPref, baggageWeight);
    }

}