using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;
using Passenger.Services;

namespace Passenger.Models;

public class Passenger
{
    public Guid PassengerId {get; private set;}
    public IEnumerable<string> MealPreference {get; private set;}
    public List<string> MealChoicesBecauseImDumb {get; private set;}
    public string MealChoice{get;private set;}
    public float BaggageWeight {get; private set;}
    public Guid? TicketId {get;private set; }
    public PassengerClass PassengerClass {get; private set;}
    public PassengerStatus Status {get; private set;}
    public DateTime BoardingStart {get;private set;}
    private IPassengerInteractionService _interactionService;
    private ILogger<Passenger> _logger;
    private static readonly Random _random = new();
    private Passenger(IPassengerInteractionService interactionService, ILogger<Passenger> logger)
    {
        _interactionService = interactionService;
        _logger = logger;
    }
    
    public static Passenger CreateInAirportStartingPoint(IPassengerInteractionService interactionService, IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass, ILogger<Passenger> logger)
    {
        Passenger passenger = new Passenger(interactionService, logger)
        {
            PassengerId = Guid.NewGuid(),
            MealPreference = mealPref,
            MealChoicesBecauseImDumb = mealPref.ToList(),
            BaggageWeight = baggageWeight,
            PassengerClass = passengerClass,
            TicketId = null,
            Status = PassengerStatus.AwaitingTicket
        };

        return passenger;
    }

    public static Passenger CreateOnPlane(IPassengerInteractionService interactionService, IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass, ILogger<Passenger> logger)
    {
        Passenger passenger = new Passenger(interactionService, logger)
        {
            PassengerId = Guid.NewGuid(),
            MealPreference = mealPref,
            MealChoicesBecauseImDumb = mealPref.ToList(),
            BaggageWeight = baggageWeight,
            TicketId = null,
            PassengerClass = passengerClass,
            Status = PassengerStatus.OnPlane
        };

        return passenger;
    }

    public async Task<bool> BuyTicket(string flightId)
    {
        MealChoice = MealChoicesBecauseImDumb[_random.Next(MealChoicesBecauseImDumb.Count)];

        var request = new BuyTicketRequest{
            PassengerId = PassengerId.ToString(),
            FlightId = flightId.ToString(),
            SeatClass = PassengerClass.ToString(),
            MealType = MealChoice,
            Baggage = BaggageWeight>0.0f ? "да" : "нет"
        };

        var response = await _interactionService.BuyTicketAsync(request);

        if(!response.IsSuccessful) 
        {
            _logger.LogWarning($"Failed to buy ticket: FlightID: {flightId}; PassengerID: {PassengerId}; Error: {response.ErrorMessage}");
            return false;
        }
        
        TicketId = response.Data!.TicketId;

        Status = PassengerStatus.AwaitingRegistration;
        return true;
    }

    public async Task<bool> RegisterForFlight()
    {

        var request = new RegisterPassengerRequest{
            PassengerId = PassengerId,
            BaggageWeight = BaggageWeight,
            MealType = MealChoice
        };
        
        var response = await _interactionService.RegisterPassengerAsync(request);

        if(!response.IsSuccessful) 
        {
            _logger.LogWarning($"Failed to register passenger: PassengerID: {PassengerId}; Error: {response.ErrorMessage}");
            return false;
        }

        BoardingStart = response.Data!.StartPlantingTime;

        Status = PassengerStatus.AwaitingBoarding;
        return true;
    }

    public async Task<bool> AttemptBoarding()
    {
        if(BoardingStart<=DateTime.Now)
        {
            _logger.LogInformation($"Passenger with the ID {PassengerId} is waiting to board their plane");
            await Task.Delay(DateTime.Now-BoardingStart-TimeSpan.FromSeconds(1));
        }

        Status = PassengerStatus.InTransit;
        return true;
    }

    public static event Action<Passenger>? OnPassengerDied;
    public async Task<bool> Die()
    {
        
        OnPassengerDied?.Invoke(this);

        Status = PassengerStatus.LeavingAirport;
        return true;
    }

}