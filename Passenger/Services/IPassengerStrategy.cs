namespace Passenger.Services;

public interface IPassengerStrategy
{
    public Models.Passenger CreatePassenger(IEnumerable<string> mealPref, float baggageWeight);
    public Task ExecutePassengerAction(Models.Passenger passenger);
}