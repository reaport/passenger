using Passenger.Services;

namespace Passenger.Models;

public class PlaneStartPassengerFactory : IPassengerFactory
{
    public PlaneStartPassengerFactory(IPassengerInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    public IPassengerInteractionService _interactionService;
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass)
    {
        return Passenger.CreateOnPlane(_interactionService, mealPref, baggageWeight, passengerClass);
    }

}