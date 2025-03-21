using Passenger.Infrastructure;
using Passenger.Infrastructure.DTO;
using Passenger.Infrastructure.DTO.HUETA;
using Passenger.Services;

namespace Passenger.Models;

public class Passenger
{
    public Guid PassengerId { get; private set; }
    public Guid? TicketId { get; private set; }
    public string MealChoice { get; private set; }
    public float BaggageWeight { get; private set; }
    public string PassengerClass { get; private set; }
    public PassengerStatus Status { get; private set; }
    public FlightInfo FlightInfo {get; private set;}
    private readonly int MaxRetries = 3;
    private readonly IPassengerStrategy _strategy;
    private readonly InteractionServiceHolder _interactionServiceResolver;
    private readonly ILoggingService _logger;
    private static readonly Random _random = new();

    private Passenger(InteractionServiceHolder interactionServiceResolver, ILoggingService logger, IPassengerStrategy passengerStrategy)
    {
        _strategy = passengerStrategy;
        _interactionServiceResolver = interactionServiceResolver;
        _logger = logger;
    }

    public static Passenger CreateInAirportStartingPoint(IPassengerStrategy strategy, InteractionServiceHolder interactionService, float baggageWeight, string passengerClass, ILoggingService logger, FlightInfo flightInfo)
    {
        return new Passenger(interactionService, logger, strategy)
        {
            PassengerId = Guid.NewGuid(),
            BaggageWeight = baggageWeight,
            PassengerClass = passengerClass,
            FlightInfo = flightInfo,
            MealChoice = flightInfo.AvailableMealTypes.ElementAt(_random.Next(0, flightInfo.AvailableMealTypes.Count())),
            TicketId = null,
            Status = PassengerStatus.AwaitingTicket
        };
    }
    public static Passenger CreateOnPlane(IPassengerStrategy strategy, InteractionServiceHolder interactionService, float baggageWeight, string passengerClass, ILoggingService logger, FlightInfo flightInfo)
    {
        return new Passenger(interactionService, logger, strategy)
        {
            PassengerId = Guid.NewGuid(),
            BaggageWeight = baggageWeight,
            PassengerClass = passengerClass,
            FlightInfo = flightInfo,
            MealChoice = flightInfo.AvailableMealTypes.ElementAt(_random.Next(0, flightInfo.AvailableMealTypes.Count())),
            TicketId = null,
            Status = PassengerStatus.OnPlane
        };
    }

    public async Task<bool> ExecuteNextStep()
    {
        bool retreived = _strategy.TryRetreiveNextPassengerStep(out var step);

        if (!retreived) return false;

        int attempt = 0;
        while(attempt<MaxRetries)
        {
            if(await step(this)) return true;

            _logger.Log<Passenger>(LogLevel.Warning, $"Passenger {PassengerId} failed their action, attempt {attempt}, retrying");
            attempt++;
        }
        _logger.Log<Passenger>(LogLevel.Warning, $"Passenger {PassengerId} failed all retries");
        return false;
    }

    public async Task<bool> BuyTicket(string flightId)
    {
        var request = new BuyTicketRequest
        {
            PassengerId = PassengerId.ToString(),
            FlightId = flightId.ToString(),
            SeatClass = char.ToUpper(PassengerClass.ToString()[0]) + PassengerClass.ToString().Substring(1),
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
        TimeSpan delay = registrationStart.ToLocalTime() - DateTime.Now;
        _logger.Log<Passenger>(LogLevel.Debug, $"Passenger {PassengerId} is waiting to register for flight {FlightInfo.FlightId} until {registrationStart.ToString("o")}");
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay);
        }

        _logger.Log<Passenger>(LogLevel.Debug, $"Passenger {PassengerId} is done waiting for registration {FlightInfo.FlightId}, registering...");

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

        FlightInfo.BoardingStart = response.Data!.StartPlantingTime;

        Status = PassengerStatus.AwaitingBoarding;
        _logger.Log<Passenger>(LogLevel.Information, $"Passenger {PassengerId} succesfully registered for their flight");
        return true;
    }

    public async Task<bool> AttemptBoarding()
    {
        var now = DateTime.UtcNow;
        if (FlightInfo.BoardingStart > now)
        {
            var delay = FlightInfo.BoardingStart - now - TimeSpan.FromSeconds(1);
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);
            }

        }

        _logger.Log<Passenger>(LogLevel.Information, $"Passenger {PassengerId} boarded");
        Status = PassengerStatus.InTransit;
        return true;
    }

    public async Task<bool> Depart()
    {
        var now = DateTime.UtcNow;

        if(FlightInfo.DepartureTime > now)
        {
            var delay = FlightInfo.BoardingStart - now;
            await Task.Delay(delay);
        }

        _logger.Log<Passenger>(LogLevel.Information, $"Passenger {PassengerId} has successfully departed");
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
