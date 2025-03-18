using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;
using Passenger.Services;

namespace Passenger.Models;

public class Passenger
{
    public Guid PassengerId { get; private set; }
    public IEnumerable<string> MealPreference { get; private set; }
    public List<string> MealChoicesBecauseImDumb { get; private set; }
    public string MealChoice { get; private set; }
    public float BaggageWeight { get; private set; }
    public Guid? TicketId { get; private set; }
    public PassengerClass PassengerClass { get; private set; }
    public PassengerStatus Status { get; private set; }
    public DateTime BoardingStart { get; private set; }
    private readonly InteractionServiceHolder _interactionServiceResolver;
    private readonly ILoggingService _logger;
    private static readonly Random _random = new();

    private Passenger(InteractionServiceHolder interactionServiceResolver, ILoggingService logger)
    {
        _interactionServiceResolver = interactionServiceResolver;
        _logger = logger;
    }

    public static Passenger CreateInAirportStartingPoint(InteractionServiceHolder interactionService, IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass, ILoggingService logger)
    {
        return new Passenger(interactionService, logger)
        {
            PassengerId = Guid.NewGuid(),
            MealPreference = mealPref,
            MealChoicesBecauseImDumb = mealPref.ToList(),
            BaggageWeight = baggageWeight,
            PassengerClass = passengerClass,
            TicketId = null,
            Status = PassengerStatus.AwaitingTicket
        };
    }

    public static Passenger CreateOnPlane(InteractionServiceHolder interactionService, IEnumerable<string> mealPref, float baggageWeight, PassengerClass passengerClass, ILoggingService logger)
    {
        return new Passenger(interactionService, logger)
        {
            PassengerId = Guid.NewGuid(),
            MealPreference = mealPref,
            MealChoicesBecauseImDumb = mealPref.ToList(),
            BaggageWeight = baggageWeight,
            TicketId = null,
            PassengerClass = passengerClass,
            Status = PassengerStatus.OnPlane
        };
    }

    public async Task<bool> BuyTicket(string flightId)
    {
        MealChoice = MealChoicesBecauseImDumb[_random.Next(MealChoicesBecauseImDumb.Count)];

        var request = new BuyTicketRequest
        {
            PassengerId = PassengerId.ToString(),
            FlightId = flightId.ToString(),
            SeatClass = PassengerClass.ToString(),
            MealType = MealChoice,
            Baggage = BaggageWeight > 0.0f ? "да" : "нет"
        };

        var service = _interactionServiceResolver.GetService();
        if (service == null)
        {
            _logger.Log<Passenger>(LogLevel.Error, "Interaction service is null. Cannot buy ticket.");
            return false;
        }

        var response = await service.BuyTicketAsync(request); 

        if (!response.IsSuccessful)
        {
            _logger.Log<Passenger>(LogLevel.Warning, $"Failed to buy ticket: FlightID: {flightId}; PassengerID: {PassengerId}; Error: {response.ErrorMessage}");
            return false;
        }

        TicketId = response.Data!.TicketId;
        Status = PassengerStatus.AwaitingRegistration;
        _logger.Log<Passenger>(LogLevel.Information, $"Passenger {PassengerId} succesfully bought their ticket");
        return true;
    }

    public async Task<bool> RegisterForFlight(DateTime registrationStart)
    {
        TimeSpan delay = registrationStart - DateTime.UtcNow;
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay);
        }

        var request = new RegisterPassengerRequest
        {
            PassengerId = PassengerId,
            BaggageWeight = BaggageWeight,
            MealType = MealChoice
        };

        var service = _interactionServiceResolver.GetService();
        if (service == null)
        {
            _logger.Log<Passenger>(LogLevel.Error, "Interaction service is null. Cannot register for flight.");
            return false;
        }

        var response = await service.RegisterPassengerAsync(request);

        if (!response.IsSuccessful)
        {
            _logger.Log<Passenger>(LogLevel.Warning, $"Failed to register passenger: PassengerID: {PassengerId}; Error: {response.ErrorMessage}");
            return false;
        }

        BoardingStart = response.Data!.StartPlantingTime;
        Status = PassengerStatus.AwaitingBoarding;
        _logger.Log<Passenger>(LogLevel.Information, $"Passenger {PassengerId} succesfully registered for their flight");
        return true;
    }

    public async Task<bool> AttemptBoarding()
    {
        var now = DateTime.UtcNow; // Use UTC time
        if (BoardingStart > now)
        {
            var delay = BoardingStart - now - TimeSpan.FromSeconds(1);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);
            }
        }

        _logger.Log<Passenger>(LogLevel.Information, $"Passenger {PassengerId} boarded");
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
