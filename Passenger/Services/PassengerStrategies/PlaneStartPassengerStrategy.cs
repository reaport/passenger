namespace Passenger.Services;

public class PlaneStartPassengerStrategy : IPassengerStrategy
{
    private readonly Queue<Func<Models.Passenger, Task<bool>>> _passengerSteps;
    public Guid FlightId { get; set; }
    public const int MaxRetries = 3;

    public PlaneStartPassengerStrategy(Guid flightId)
    {
        FlightId = flightId;

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
        }
    }
}