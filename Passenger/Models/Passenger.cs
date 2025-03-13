using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Passenger.Services;

namespace Passenger.Models;

public class Passenger
{
    public Guid PassengerId {get; private set;}
    public IEnumerable<string> MealPreference {get; private set;}
    public float BaggageWeight {get; private set;}
    public Guid? TicketId {get;private set; }
    public PassengerClass PassengerClass {get; private set;}
    public PassengerStatus Status {get; private set;}
    private IPassengerInteractionService _interactionService;
    private Passenger(IPassengerInteractionService interactionService)
    {
        _interactionService = interactionService;
    }
    
    public static Passenger CreateInAirportStartingPoint(IPassengerInteractionService interactionService, IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass)
    {
        Passenger passenger = new Passenger(interactionService)
        {
            PassengerId = new Guid(),
            MealPreference = mealPref,
            BaggageWeight = baggageWeight,
            PassengerClass = passengerClass,
            TicketId = null,
            Status = PassengerStatus.AwaitingTicket
        };

        return passenger;
    }

    public static Passenger CreateOnPlane(IPassengerInteractionService interactionService, IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass)
    {
        Passenger passenger = new Passenger(interactionService)
        {
            PassengerId = new Guid(),
            MealPreference = mealPref,
            BaggageWeight = baggageWeight,
            TicketId = null,
            PassengerClass = passengerClass,
            Status = PassengerStatus.OnPlane
        };

        return passenger;
    }

    public async Task<bool> BuyTicket(Guid flightId)
    {
        Status = PassengerStatus.AwaitingRegistration;
        return true;
    }

    public async Task<bool> RegisterForFlight()
    {
        Status = PassengerStatus.AwaitingBoarding;
        return true;
    }

    public async Task<bool> AttemptBoarding()
    {
        Status = PassengerStatus.InTransit;
        return true;
    }

    public async Task Die()
    {
        // ded
    }

}