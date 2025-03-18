using System.Threading.Tasks;
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
        private IPassengerService _passengerService;
        private RefreshServiceHolder _refreshServiceHolder;
        private InteractionServiceHolder _interactionServiceHolder;

        public HomeController(FlightController flightController, DriverController driverController, IPassengerService passengerService, IDriverService driverService, ILoggingService loggingService, ILogger<DriverController> driverLogger, InteractionServiceHolder interactionServiceHolder, RefreshServiceHolder refreshServiceHolder)
        {
            _flightController = flightController;
            _driverController = driverController;
            _refreshServiceHolder = refreshServiceHolder;
            _interactionServiceHolder = interactionServiceHolder;
            _loggingService = loggingService;
            _passengerService = passengerService;
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

            if (flights == null || flights.Count == 0)
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

        public async Task<IActionResult> PauseDriverService()
        {
            var result = await _driverController.PauseDriverService() as OkObjectResult;
            ViewBag.Message = result?.Value?.ToString();

            return View("Index");
        }

        public async Task<IActionResult> ResumeDriverService()
        {
            var result = await _driverController.ResumeDriverService() as OkObjectResult;
            ViewBag.Message = result?.Value?.ToString();

            return View("Index");
        }

        public async Task<IActionResult> UseFakeData()
        {
            try
            {
                await _driverController.PauseDriverService();
                _refreshServiceHolder.SetService("fake");
                _interactionServiceHolder.SetService("fake");
                _passengerService.CleanupFlights();
                await _driverController.ResumeDriverService();
                _loggingService.Log<HomeController>(LogLevel.Information, $"Successfully switched to fake data");
                ViewBag.Message = $"Set fake data";
            }
            catch (Exception e)
            {
                _loggingService.Log<HomeController>(LogLevel.Information, $"Failed to switch to fake data");
                ViewBag.Message = $"Failed to set fake data";
                throw;
            }
            return View("Index");
        }

        public async Task<IActionResult> UseRealDataAsync()
        {
            try
            {
                await _driverController.PauseDriverService();
                _refreshServiceHolder.SetService("real");
                _interactionServiceHolder.SetService("real");
                _passengerService.CleanupFlights();
                await _driverController.ResumeDriverService();
                _loggingService.Log<HomeController>(LogLevel.Information, $"Successfully switched to real data");
                ViewBag.Message = $"Set real data";
            }
            catch (Exception e)
            {
                _loggingService.Log<HomeController>(LogLevel.Error, $"Failed to switch to real data");
                ViewBag.Message = $"Failed to set real data";
                throw;
            }
            return View("Index");
        }
    }
}