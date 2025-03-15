using Microsoft.AspNetCore.Mvc;
using Passenger.Controllers;
using Passenger.Services;
using Microsoft.Extensions.Logging;
using Passenger.Infrastructure.DTO;

namespace Passenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly FlightController _flightController;
        private readonly DriverController _driverController;

        public HomeController(IPassengerService passengerService, IDriverService driverService, ILogger<DriverController> logger)
        {
            _flightController = new FlightController(passengerService);
            _driverController = new DriverController(driverService, logger);
        }

        public IActionResult Index()
        {
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

            return View("Index", passengers); // Pass the data to the view
        }

        public IActionResult PauseDriverService()
        {
            var result = _driverController.PauseDriverService() as OkObjectResult;
            ViewBag.Message = result?.Value?.ToString(); // Pass the message to the view
            return View("Index");
        }

        public IActionResult ResumeDriverService()
        {
            var result = _driverController.ResumeDriverService() as OkObjectResult;
            ViewBag.Message = result?.Value?.ToString(); // Pass the message to the view
            return View("Index");
        }
    }
}