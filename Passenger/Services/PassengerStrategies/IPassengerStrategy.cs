namespace Passenger.Services;

public interface IPassengerStrategy : ICloneable
{
    public bool TryRetreiveNextPassengerStep(out Func<Models.Passenger, Task<bool>> step);
}