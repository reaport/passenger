namespace Passenger.Services;

public class RefreshServiceHolder
{
    private readonly IServiceProvider _serviceProvider;
    private IFlightRefreshService _currentService;

    public RefreshServiceHolder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _currentService = _serviceProvider.GetRequiredKeyedService<IFlightRefreshService>("real"); // Default service
    }

    public IFlightRefreshService GetService() => _currentService;

    public void SetService(string serviceKey)
    {
        _currentService = _serviceProvider.GetRequiredKeyedService<IFlightRefreshService>(serviceKey);
    }
}