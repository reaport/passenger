using Microsoft.AspNetCore.Mvc;
using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlightController _flightController;
        private readonly DriverController _driverController;
        private readonly ILoggingService _loggingService;

        public HomeController(IPassengerService passengerService, IDriverService driverService, ILoggingService loggingService, ILogger<DriverController> driverLogger)
        {
            _flightController = new FlightController(passengerService);
            _driverController = new DriverController(driverService, driverLogger);
            _loggingService = loggingService;
        }

        public IActionResult Index()
        {
            ViewBag.Logs = _loggingService.GetLogs();
            return View();
        }

        public IActionResult GetInitialisedFlights()
        {
            var result = _flightController.GetInitialisedFlights();
            var flights = result?.Value;

            if (flights == null || !flights.Any())
            {
                ViewBag.Message = "No flights available.";
            }

            return View("Index", flights);
        }

        public IActionResult GetPassengersPerFlight()
        {
            var result = _flightController.GetPassengersPerFlight();
            var passengers = result?.Value;

            if (passengers == null || !passengers.Any())
            {
                ViewBag.Message = "No passengers data available.";
            }

            return View("Index", passengers);
        }

        public IActionResult GetLogs()
        {
            var logs = _loggingService.GetLogs();
            return Json(logs); 
        }

        public IActionResult PauseDriverService()
        {
            var result = _driverController.PauseDriverService() as OkObjectResult;
            ViewBag.Message = result?.Value?.ToString();

            return View("Index");
        }

        public IActionResult ResumeDriverService()
        {
            var result = _driverController.ResumeDriverService() as OkObjectResult;
            ViewBag.Message = result?.Value?.ToString();

            return View("Index");
        }
    }
}