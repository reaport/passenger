using Microsoft.AspNetCore.Mvc;
using Passenger.Services;

namespace Passenger.Controllers;

[Route("/driver")]
public class DriverController : ControllerBase
{
    private readonly IDriverService _driverService;

    public DriverController(IDriverService driverService)
    {
        _driverService = driverService;
    }

    [HttpPost("pause")]
    public ActionResult PauseDriverService()
    {
        _driverService.Pause();
        return Ok("Driver service paused.");
    }

    [HttpPost("resume")]
    public ActionResult ResumeDriverService()
    {
        _driverService.Resume();
        return Ok("Driver service resumed.");
    }
}