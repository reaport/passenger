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
            else
            {
                _loggingService.Log("Retrieved initialised flights.");
            }

            ViewBag.Logs = _loggingService.GetLogs();
            return View("Index", flights);
        }

        public IActionResult GetPassengersPerFlight()
        {
            var result = _flightController.GetPassengersPerFlight();
            var passengers = result?.Value;

            if (passengers == null || !passengers.Any())
            {
                //_loggingService.Log("No passengers data available.");
                ViewBag.Message = "No passengers data available.";
            }
            else
            {
                _loggingService.Log("Retrieved passengers per flight.");
            }

            ViewBag.Logs = _loggingService.GetLogs();
            return View("Index", passengers);
        }

        public IActionResult GetLogs()
        {
            var logs = _loggingService.GetLogs();
            return Json(logs); // Return logs as JSON
        }

        public IActionResult PauseDriverService()
        {
            var result = _driverController.PauseDriverService() as OkObjectResult;
            _loggingService.Log("Driver service paused.");
            ViewBag.Message = result?.Value?.ToString();
            ViewBag.Logs = _loggingService.GetLogs();
            return View("Index");
        }

        public IActionResult ResumeDriverService()
        {
            var result = _driverController.ResumeDriverService() as OkObjectResult;
            _loggingService.Log("Driver service resumed.");
            ViewBag.Message = result?.Value?.ToString();
            ViewBag.Logs = _loggingService.GetLogs();
            return View("Index");
        }
    }
}