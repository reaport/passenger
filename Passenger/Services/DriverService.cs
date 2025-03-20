using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace Passenger.Services;

public class DriverService : IDriverService, IDisposable
{
    private readonly ILoggingService _logger;
    private readonly IPassengerService _passengerService;
    private bool _isPaused = true; // No longer volatile - protected by semaphore
    private readonly SemaphoreSlim _pauseSemaphore = new SemaphoreSlim(1, 1);
    public PeriodicTimer _refreshTimer;
    public DriverService(ILoggingService logger, IPassengerService passengerService)
    {
        _logger = logger;
        _passengerService = passengerService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.Log<DriverService>(LogLevel.Information, "Background driving service started running.");
        _refreshTimer = new PeriodicTimer(TimeSpan.FromSeconds(3));
        
        // Use Task.Run instead of StartNew for clearer async intent
        Task.Run(() => DoTimedTasks(_refreshTimer, stoppingToken), stoppingToken);
        
        return Task.CompletedTask;
    }

    private async Task DoTimedTasks(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            // Async-friendly synchronization
            await _pauseSemaphore.WaitAsync(cancellationToken);
            try
            {
                if (_isPaused)
                {
                    continue;
                }
            }
            finally
            {
                _pauseSemaphore.Release();
            }

            try
            {
                await _passengerService.RefreshAndInitFlights();
                _logger.Log<DriverService>(LogLevel.Information, "Refreshed available flights.");
            }
            catch (Exception e)
            {
                _logger.Log<DriverService>(LogLevel.Critical, 
                    $"Could not refresh flights: {e.Message}");
                await Task.Delay(5000, cancellationToken); 
                continue;
            }

            try
            {
                _logger.Log<DriverService>(LogLevel.Debug, "Trying to execute passenger actions");
                await _passengerService.ExecutePassengerActions();
            }
            catch (Exception e)
            {
                _logger.Log<DriverService>(LogLevel.Critical, 
                    $"Unhandled exception executing passenger actions: {e.Message}\n{e.StackTrace}");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.Log<DriverService>(LogLevel.Information, 
            "The passengers become motionless husks as the driver service finishes execution.");
        return Task.CompletedTask;
    }

    public async Task Pause()
    {
        await _pauseSemaphore.WaitAsync();
        try { _isPaused = true; }
        finally { _pauseSemaphore.Release(); }
        _logger.Log<DriverService>(LogLevel.Information, "Driver service paused");

    }

    public async Task Resume()
    {
        await _pauseSemaphore.WaitAsync();
        try { _isPaused = false; }
        finally { _pauseSemaphore.Release(); }
        _logger.Log<DriverService>(LogLevel.Information, "Driver service resumed");

    }

    public async Task CleanUpFlights()
    {
        await Pause();
        try
        {
            _passengerService.CleanupFlights();
        }
        finally
        {
            await Resume();
        }
    }

    public void Dispose()
    {
        _refreshTimer?.Dispose();
        _pauseSemaphore?.Dispose(); // Clean up semaphore
    }
}