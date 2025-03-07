namespace Passenger.Services;

public class PlaneStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task>> _passengerSteps;
    public async Task ExecutePassengerAction(Models.Passenger passenger)
    {
        if(_passengerSteps.Count > 0)
        {
            var currentStep = _passengerSteps.Dequeue();
            await currentStep(passenger);
        }
    }
}