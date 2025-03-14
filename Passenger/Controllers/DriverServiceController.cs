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
    public ActionResult PauseDriverService()
    {
        _driverService.Pause();
        _logger.LogInformation("Paused the driver service");
        return Ok("Driver service paused.");
    }

    [HttpPost("resume")]
    public ActionResult ResumeDriverService()
    {
        _driverService.Resume();
        _logger.LogInformation("Resumed the driver service");
        return Ok("Driver service resumed.");
    }
}