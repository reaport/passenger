using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Passenger.Services;

public class DriverService : IDriverService, IDisposable
{
    private readonly ILoggingService _logger;
    private readonly IPassengerService _passengerService;
    private bool _isPaused = false;
    public PeriodicTimer _flightRefreshTimer;
    public PeriodicTimer _passengerActionsTimer;

    public DriverService(ILoggingService logger, IPassengerService passengerService)
    {
        _logger = logger;
        _passengerService = passengerService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.Log<DriverService>(LogLevel.Information, "Background driving service started running.");

        _flightRefreshTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        _passengerActionsTimer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        Task.Factory.StartNew(() => RefreshAvailableFlights(_flightRefreshTimer, stoppingToken), stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        Task.Factory.StartNew(() => ExecutePassengerBullshit(_passengerActionsTimer, stoppingToken), stoppingToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

        return Task.CompletedTask;
    }

    private async Task RefreshAvailableFlights(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                await _passengerService.RefreshAndInitFlights();
                _logger.Log<DriverService>(LogLevel.Information, "Refreshed available flights.");
            }
            catch(Exception e)
            {
                _logger.Log<DriverService>(LogLevel.Critical, $"unhandled exception in driver service: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    private async Task ExecutePassengerBullshit(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            _logger.Log<DriverService>(LogLevel.Debug, $"ExecutePassengerBullshitTimerFired");
            try
            {
                await _passengerService.ExecutePassengerActions();
            }
            catch (Exception e)
            {
                _logger.Log<DriverService>(LogLevel.Critical, $"unhandled exception in driver service passenger bullshit: {e.Message}\n{e.StackTrace}");
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.Log<DriverService>(LogLevel.Information, "The passengers become motionless husks as the driver service finishes execution.");
        return Task.CompletedTask;
    }

    public void Pause()
    {
        _isPaused = true;
    }

    public void Resume()
    {
        _isPaused = false;
    }

    public void Dispose()
    {
        _flightRefreshTimer?.Dispose();
        _passengerActionsTimer?.Dispose();
    }
}