using Passenger.Services;

namespace Passenger.Models;

public class AirportStartPassengerFactory : IPassengerFactory
{
    public IPassengerInteractionService _interactionService;
    public AirportStartPassengerFactory(IPassengerInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight)
    {
        return Passenger.CreateInAirportStartingPoint(_interactionService, mealPref, baggageWeight);
    }

}