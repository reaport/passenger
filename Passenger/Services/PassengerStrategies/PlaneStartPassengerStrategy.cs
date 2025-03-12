namespace Passenger.Services;

public class PlaneStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task>> _passengerSteps;

    public Guid FlightId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public PlaneStartPassengerStrategy(Guid flightId)
    {
        FlightId = flightId;

        _passengerSteps = new(1);

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