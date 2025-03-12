
using Passenger.Models;

namespace Passenger.Services;

public class AirportStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task>> _passengerSteps;
    public Guid FlightId { get; set; }
    public AirportStartPassengerStrategy(Guid flightId)
    {
        FlightId = flightId;

        _passengerSteps = new(4);

        _passengerSteps.Enqueue( p=>p.BuyTicket(flightId));
        _passengerSteps.Enqueue(p=>p.RegisterForFlight());
        _passengerSteps.Enqueue(p=>p.AttemptBoarding());
        _passengerSteps.Enqueue(p=>p.Die());
        
    }
    public async Task ExecutePassengerAction(Models.Passenger passenger)
    {
        if(_passengerSteps.Count > 0)
        {
            var currentStep = _passengerSteps.Dequeue();
            await currentStep(passenger);
        }
    }
}