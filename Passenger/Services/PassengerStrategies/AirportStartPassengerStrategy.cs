
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Passenger.Infrastructure.DTO;
using Passenger.Models;

namespace Passenger.Services;

public class AirportStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task<bool>>> _passengerSteps;
    private FlightInfo _flightInfo;
    private ILoggingService _logger;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    private const int MaxRetries = 3;
    public AirportStartPassengerStrategy(FlightInfo flightInfo, ILoggingService logger)
    {
        _flightInfo = flightInfo;
        _logger = logger;

        _passengerSteps = new(4);

        _passengerSteps.Enqueue( async p =>
        {
            _logger.Log<AirportStartPassengerStrategy>(LogLevel.Debug, $"Passanger {p.PassengerId} is buying a ticket for flight {_flightInfo.FlightId}");
            return await p.BuyTicket(_flightInfo.FlightId);
        });

        _passengerSteps.Enqueue(async p=>
        {
            _logger.Log<AirportStartPassengerStrategy>(LogLevel.Debug, $"Passanger {p.PassengerId} is registering for the flight {_flightInfo.FlightId}");
            return await p.RegisterForFlight(_flightInfo.RegistrationStartTime);
        });
        
        _passengerSteps.Enqueue(async p=>
        {
            _logger.Log<AirportStartPassengerStrategy>(LogLevel.Debug, $"Passanger {p.PassengerId} is boarding flight {_flightInfo.FlightId}");
            return await p.AttemptBoarding();
        });
        
        _passengerSteps.Enqueue(async p=>
        {
            _logger.Log<AirportStartPassengerStrategy>(LogLevel.Debug, $"Passanger {p.PassengerId} is leaving the aiport");
            return await p.Die();
        });
        
    }
    public async Task<bool> ExecutePassengerAction(Models.Passenger passenger)
{
    await _semaphore.WaitAsync();
    try
    {
        if (_passengerSteps.Count == 0) return false;
        var currentStep = _passengerSteps.Peek();

        for (int attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                bool success = await currentStep(passenger);
                if (success)
                {
                    _passengerSteps.Dequeue();
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.Log<AirportStartPassengerStrategy>(LogLevel.Error, $"Step failed (attempt {attempt + 1}): {ex.Message}");
            }
            
            await Task.Delay(1000);
        }
        
        _logger.Log<AirportStartPassengerStrategy>(LogLevel.Warning, $"Passenger {passenger.PassengerId} failed all retries");
        return await passenger.Die();
    }
    finally
    {
        _semaphore.Release();
    }
}

}