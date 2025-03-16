using Passenger.Services;

namespace Passenger.Models;

public class AirportStartPassengerFactory : IPassengerFactory
{
    public IPassengerInteractionService _interactionService;
    public ILoggingService _passengerLogger;
    public AirportStartPassengerFactory(IPassengerInteractionService interactionService, ILoggingService logger)
    {
        _interactionService = interactionService;
        _passengerLogger = logger;
    }
    public Passenger Create(IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass)
    {
        return Passenger.CreateInAirportStartingPoint(_interactionService, mealPref, baggageWeight, passengerClass, _passengerLogger);
    }

}