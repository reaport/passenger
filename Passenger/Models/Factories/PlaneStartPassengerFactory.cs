using Passenger.Services;

namespace Passenger.Models;

public class PlaneStartPassengerFactory : IPassengerFactory
{
    public PlaneStartPassengerFactory(IPassengerInteractionService interactionService, ILoggingService logger)
    {
        _interactionService = interactionService;
        _passengerLogger = logger;
    }
    public IPassengerInteractionService _interactionService;
    public ILoggingService _passengerLogger;
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass)
    {
        return Passenger.CreateOnPlane(_interactionService, mealPref, baggageWeight, passengerClass, _passengerLogger);
    }

}