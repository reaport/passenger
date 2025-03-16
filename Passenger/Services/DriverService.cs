using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Passenger.Services;

public class DriverService : IDriverService, IDisposable
{
    private readonly ILoggingService _logger;
    private readonly IPassengerService _passengerService;
    private readonly object _lock = new object();
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
        _passengerActionsTimer = new PeriodicTimer(TimeSpan.FromSeconds(2));

        Task.Run(() => RefreshAvailableFlights(_flightRefreshTimer, stoppingToken), stoppingToken);
        Task.Run(() => ExecutePassengerBullshit(_passengerActionsTimer, stoppingToken), stoppingToken);

        return Task.CompletedTask;
    }

    private async Task RefreshAvailableFlights(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            lock (_lock)
            {
                while (_isPaused)
                {
                    Monitor.Wait(_lock);
                }
            }

            await _passengerService.RefreshAndInitFlights();
            _logger.Log<DriverService>(LogLevel.Information, "Refreshed available flights.");
        }
    }

    private async Task ExecutePassengerBullshit(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            lock (_lock)
            {
                while (_isPaused)
                {
                    Monitor.Wait(_lock);
                }
            }

            await _passengerService.ExecutePassengerActions();
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.Log<DriverService>(LogLevel.Information, "The passengers become motionless husks as the driver service finishes execution.");
        return Task.CompletedTask;
    }

    public void Pause()
    {
        lock (_lock)
        {
            _isPaused = true;
        }
    }

    public void Resume()
    {
        lock (_lock)
        {
            _isPaused = false;
            Monitor.PulseAll(_lock); // Notify all waiting threads to resume
        }
    }

    public void Dispose()
    {
        _flightRefreshTimer?.Dispose();
        _passengerActionsTimer?.Dispose();
    }
}