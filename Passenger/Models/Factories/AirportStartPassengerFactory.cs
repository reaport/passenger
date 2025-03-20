using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Models;

public class AirportStartPassengerFactory : IPassengerFactory
{
    public InteractionServiceHolder _interactionService;
    public ILoggingService _loggingService;
    public AirportStartPassengerFactory(InteractionServiceHolder interactionService, ILoggingService logger)
    {
        _interactionService = interactionService;
        _loggingService = logger;
    }
    public Passenger Create(InteractionServiceHolder serviceHolder, float baggageWeight, string passengerClass, FlightInfo flightInfo)
    {
        return Passenger.CreateInAirportStartingPoint(new AirportStartPassengerStrategy(flightInfo), serviceHolder, baggageWeight, passengerClass, _loggingService, flightInfo);
    }

}