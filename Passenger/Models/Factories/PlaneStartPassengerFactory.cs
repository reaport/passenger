using Passenger.Services;

namespace Passenger.Models;

public class PlaneStartPassengerFactory : IPassengerFactory
{
    public PlaneStartPassengerFactory(InteractionServiceHolder interactionService, ILoggingService logger)
    {
        _interactionService = interactionService;
        _passengerLogger = logger;
    }
    public InteractionServiceHolder _interactionService;
    public ILoggingService _passengerLogger;
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass)
    {
        return Passenger.CreateOnPlane(_interactionService, mealPref, baggageWeight, passengerClass, _passengerLogger);
    }

}