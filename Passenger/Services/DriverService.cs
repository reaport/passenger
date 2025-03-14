using System.Threading.Tasks;

namespace Passenger.Services;

public class DriverService : IDriverService
{
    private readonly ILogger<DriverService> _logger;
    public PeriodicTimer _flightRefreshTimer;
    public PeriodicTimer _passengerActionsTimer;
    private IPassengerService _passengerService;

    public DriverService(ILogger<DriverService> logger, IPassengerService passengerService)
    {
        _logger = logger;
        _passengerService = passengerService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background driving service started running.");

        _flightRefreshTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        _passengerActionsTimer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        Task.Run( () => RefreshAvailableFlights(_flightRefreshTimer, stoppingToken), stoppingToken);
        Task.Run( () => ExecutePassengerBullshit(_passengerActionsTimer, stoppingToken), stoppingToken);

        return Task.CompletedTask;
    }

    private async Task RefreshAvailableFlights(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync() && !cancellationToken.IsCancellationRequested)
        {
            await _passengerService.RefreshAndInitFlights();
            _logger.LogInformation($"Refreshed available flights.");
        }
    }

    private async Task ExecutePassengerBullshit(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while(await timer.WaitForNextTickAsync() && !cancellationToken.IsCancellationRequested)
        {
            await _passengerService.ExecutePassengerActions();
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("The passengers become motionless husks as the driver service finishes execution.");

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _flightRefreshTimer.Dispose();
        _passengerActionsTimer.Dispose();
    }
}