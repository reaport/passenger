
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Passenger.Infrastructure.DTO;
using Passenger.Models;

namespace Passenger.Services;

public class AirportStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task<bool>>> _passengerSteps;
    private FlightInfo _flightInfo;
    private const int MaxRetries = 3;
    public AirportStartPassengerStrategy(FlightInfo flightInfo)
    {
        _flightInfo = flightInfo;

        _passengerSteps = new(4);

        _passengerSteps.Enqueue( p=>p.BuyTicket(_flightInfo.FlightId));
        _passengerSteps.Enqueue(p=>p.RegisterForFlight(_flightInfo.RegistrationStartTime));
        _passengerSteps.Enqueue(p=>p.AttemptBoarding());
        _passengerSteps.Enqueue(p=>p.Die());
        
    }
    public async Task ExecutePassengerAction(Models.Passenger passenger)
    {
        if (_passengerSteps.Count > 0)
        {
            var currentStep = _passengerSteps.Peek();

            if(currentStep == null) return;
            
            int attempt = 0;

            while (attempt < MaxRetries)
            {
                bool success = await currentStep(passenger);
                if (success)
                {
                    _passengerSteps.Dequeue(); // Remove step only if successful
                    return;
                }
                
                attempt++;
                await Task.Delay(1000); // Retry delay
            }

            await passenger.Die();
        }
    }

}