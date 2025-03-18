using Microsoft.AspNetCore.Mvc;
using Passenger.Services;

namespace Passenger.Controllers;

[Route("/driver")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;
    private readonly ILogger<DriverController> _logger;

    public DriverController(IDriverService driverService, ILogger<DriverController> logger)
    {
        _driverService = driverService;
        _logger = logger;
    }

    [HttpPost("pause")]
    public async Task<ActionResult> PauseDriverService()
    {
        await _driverService.Pause();
        _logger.LogInformation("Paused the driver service");
        return Ok("Driver service paused.");
    }

    [HttpPost("resume")]
    public async Task<ActionResult> ResumeDriverService()
    {
        await _driverService.Resume();
        _logger.LogInformation("Resumed the driver service");
        return Ok("Driver service resumed.");
    }

    [HttpPost("clean")]
    public async Task<ActionResult> CleanUpFlights()
    {
        await _driverService.CleanUpFlights();
        return Ok("Deleted flights");
    }
}