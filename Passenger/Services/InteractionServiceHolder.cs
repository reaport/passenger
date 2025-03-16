namespace Passenger.Services;

public class InteractionServiceHolder
{
    private readonly IServiceProvider _serviceProvider;
    private IPassengerInteractionService _currentService;

    public InteractionServiceHolder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _currentService = _serviceProvider.GetRequiredKeyedService<IPassengerInteractionService>("real"); // Default service
    }

    public IPassengerInteractionService GetService() => _currentService;

    public void SetService(string serviceKey)
    {
        _currentService = _serviceProvider.GetRequiredKeyedService<IPassengerInteractionService>(serviceKey);
    }
}