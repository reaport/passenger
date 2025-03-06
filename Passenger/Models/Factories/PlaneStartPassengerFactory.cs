using Passenger.Services;

namespace Passenger.Models;

public class PlaneStartPassengerFactory : IPassengerFactory
{
    public PlaneStartPassengerFactory(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    public IInteractionService _interactionService;
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Passenger.CreateOnPlane(_interactionService, mealPref, baggageWeight);
    }

}