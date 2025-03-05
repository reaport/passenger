namespace Passenger.Models;

public class Passenger
{
    public Guid PassengerId {get; private set;}
    public IEnumerable<string> MealPreference {get; private set;}
    public float BaggageWeight {get; private set;}
    public Guid? TicketId {get;private set; }
    public PassengerStatus Status {get; private set;}
    private Passenger(){}
    
    public static Passenger CreateInAirportStartingPoint(IEnumerable<string> mealPref, float baggageWeight)
    {
        Passenger passenger = new Passenger
        {
            PassengerId = new Guid(),
            MealPreference = mealPref,
            BaggageWeight = baggageWeight,
            TicketId = null,
            Status = PassengerStatus.AwaitingTicket
        };

        return passenger;
    }

    public static Passenger CreateOnPlane(IEnumerable<string> mealPref, float baggageWeight/*, Guid ticketId*/)
    {
        Passenger passenger = new Passenger
        {
            PassengerId = new Guid(),
            MealPreference = mealPref,
            BaggageWeight = baggageWeight,
            TicketId = null,
            //TicketId = ticketId,
            Status = PassengerStatus.OnPlane
        };

        return passenger;
    }

    public async Task BuyTicket(Guid ticketId)
    {
        TicketId = ticketId;
        Status = PassengerStatus.AwaitingRegistration;
    }

    public async Task RegisterForFlight()
    {
        Status = PassengerStatus.AwaitingBoarding;
    }

    public async Task AttemptBoarding()
    {
        Status = PassengerStatus.InTransit;
    }

}