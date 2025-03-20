
using System.Collections.Concurrent;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Passenger.Infrastructure.DTO;
using Passenger.Models;

namespace Passenger.Services;

public class AirportStartPassengerStrategy : IPassengerStrategy
{
    private readonly ConcurrentQueue<Func<Models.Passenger, Task<bool>>> _passengerSteps;
    private FlightInfo _flightInfo;
    public AirportStartPassengerStrategy(FlightInfo flightInfo)
    {
        _flightInfo = flightInfo;

        _passengerSteps = new();

        _passengerSteps.Enqueue(p=> p.BuyTicket(_flightInfo.FlightId));

        _passengerSteps.Enqueue(p=> p.RegisterForFlight(_flightInfo.RegistrationStartTime));
        
        _passengerSteps.Enqueue(p=> p.AttemptBoarding());
        
        _passengerSteps.Enqueue(p=>p.Depart());

        _passengerSteps.Enqueue(p=>p.Die());
        
    }

    public object Clone()
    {
        // Create a new instance of the strategy with the cloned FlightInfo
        var clonedStrategy = new AirportStartPassengerStrategy(_flightInfo);

        return clonedStrategy;
    }

    public bool TryRetreiveNextPassengerStep(out Func<Models.Passenger, Task<bool>>? step)
    {
        var res = _passengerSteps.TryDequeue(out var currentStep);

        if(res && currentStep is not null)
        {
            step = currentStep;
            return true;
        }

        step = null;
        return false;
    }
}