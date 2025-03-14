using Microsoft.AspNetCore.Mvc;
using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Controllers;

[Route("/flights")]
public class FlightController : ControllerBase
{
    private IPassengerService _passengerService;
    public FlightController(IPassengerService passengerService)
    {
        _passengerService = passengerService;
    }

    [HttpGet("initialisedFlights")]
    public ActionResult<List<FlightInfo>> GetInitialisedFlights()
    {
        var managers = _passengerService.GetFlightManagers();

        return Ok(managers.Select(manager => manager._flightInfo));
    }
}