using Passenger.Infrastructure.DTO;
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

    public Passenger Create(InteractionServiceHolder serviceHolder, float baggageWeight, string passengerClass, FlightInfo flightInfo)
    {
        throw new NotImplementedException();
    }
}