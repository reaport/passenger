namespace Passenger.Models;

public interface IPassengerFactory
{
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight);
}