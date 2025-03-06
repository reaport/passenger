using Passenger.Services;

namespace Passenger.Models;

public class AirportStartPassengerFactory : IPassengerFactory
{
    public IInteractionService _interactionService;
    public AirportStartPassengerFactory(IInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Passenger.CreateInAirportStartingPoint(_interactionService, mealPref, baggageWeight);
    }

}