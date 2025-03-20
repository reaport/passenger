using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Models;

public interface IPassengerFactory
{
    public Passenger Create(InteractionServiceHolder serviceHolder, float baggageWeight, string passengerClass, FlightInfo flightInfo);
}