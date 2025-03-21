using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Net;

namespace Passenger.Services;

public class DriverService : IDriverService, IDisposable
{
    private readonly ILoggingService _logger;
    private readonly IPassengerService _passengerService;
    private volatile bool _isPaused = true; 
    private readonly SemaphoreSlim _pauseSemaphore = new SemaphoreSlim(1, 1);
    public PeriodicTimer _refreshTimer;
    public PeriodicTimer _passengerActionTimer;
    public DriverService(ILoggingService logger, IPassengerService passengerService)
    {
        _logger = logger;
        _passengerService = passengerService;
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.Log<DriverService>(LogLevel.Information, "Background driving service started running.");
        _refreshTimer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        Task.Run(() => RefreshFlights(_refreshTimer, stoppingToken), stoppingToken);

        return Task.CompletedTask;
    }

    private async Task RefreshFlights(PeriodicTimer timer, CancellationToken cancellationToken)
    {
        while (await timer.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {   
            if (_isPaused) continue;

            try
            {
                _logger.Log<DriverService>(LogLevel.Information, "Refreshing available flights.");
                await _passengerService.RefreshAndInitFlights();
            }
            catch (ApiRequestException e)
            {
                if(e.StatusCode == HttpStatusCode.NotFound) 
                {
                    _logger.Log<DriverService>(LogLevel.Error, $"Failed to get new flights, no flights available to buy tickets for");
                    throw;
                }
            }
            catch (Exception e)
            {
                _logger.Log<DriverService>(LogLevel.Critical, 
                    $"Could not refresh flights: {e.Message}\n{e.StackTrace}");
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
        _logger.Log<DriverService>(LogLevel.Information, "Refresh service paused");

    }

    public async Task Resume()
    {
        await _pauseSemaphore.WaitAsync();
        try { _isPaused = false; }
        finally { _pauseSemaphore.Release(); }
        _logger.Log<DriverService>(LogLevel.Information, "Refresh service resumed");

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
        _pauseSemaphore?.Dispose();
    }
}