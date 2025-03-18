
namespace Passenger.Services;

public class PlaneStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task<bool>>> _passengerSteps;
    private string _flightId;
    public const int MaxRetries = 3;

    public PlaneStartPassengerStrategy(string flightId)
    {
        _flightId = flightId;

        _passengerSteps = new(1);

        _passengerSteps.Enqueue(p=>p.Die());
    }

    public async Task ExecutePassengerAction(Models.Passenger passenger)
    {
        if (_passengerSteps.Count > 0)
        {
            var currentStep = _passengerSteps.Peek(); // Peek instead of Dequeue
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

    public void SetFlightId(string flightId)
    {
        _flightId = flightId;
    }

    public string GetFlightId()
    {
        return _flightId;
    }

    Task<bool> IPassengerStrategy.ExecutePassengerAction(Models.Passenger passenger)
    {
        throw new NotImplementedException();
    }
}