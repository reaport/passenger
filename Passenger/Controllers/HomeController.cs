using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passenger.Infrastructure.DTO;
using Passenger.Services;

namespace Passenger.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILoggingService _loggingService;
        private readonly IPassengerService _passengerService;
        private readonly IDriverService _driverService;
        private readonly RefreshServiceHolder _refreshServiceHolder;
        private readonly InteractionServiceHolder _interactionServiceHolder;

        public HomeController(
            IPassengerService passengerService,
            IDriverService driverService,
            ILoggingService loggingService,
            RefreshServiceHolder refreshServiceHolder,
            InteractionServiceHolder interactionServiceHolder)
        {
            _passengerService = passengerService;
            _driverService = driverService;
            _loggingService = loggingService;
            _refreshServiceHolder = refreshServiceHolder;
            _interactionServiceHolder = interactionServiceHolder;
        }

        public IActionResult Index()
        {
            ViewBag.Logs = _loggingService.GetLogs();
            return View();
        }

        public IActionResult GetInitialisedFlights()
        {
            var managers = _passengerService.GetFlightManagers();

            var flights = managers.Select(manager => manager._flightInfo);

            if (flights == null || !flights.Any())
            {
                ViewBag.Message = "No flights available.";
                return View("Index");
            }

            return View("Index", flights);
        }

        public IActionResult GetPassengersPerFlight()
        {
            var managers = _passengerService.GetFlightManagers();

            var items = managers.Select( manager => new PassengersPerFlight {
                FlightId = manager._flightInfo.FlightId,
                PassengerCount = manager.GetPassengerCount()
            });


            if (items == null || !items.Any())
            {
                ViewBag.Message = "No passengers data available.";
                return View("Index");
            }

            return View("Index", items);
        }

        // Keep existing GetLogs, PauseDriverService, ResumeDriverService, etc.

        public async Task<IActionResult> PauseDriverService()
        {
            await _driverService.Pause(); 
            ViewBag.Message = "Driver service paused.";
            return View("Index");
        }

        public async Task<IActionResult> ResumeDriverService()
        {
            await _driverService.Resume(); 
            ViewBag.Message = "Driver service resumed.";
            return View("Index");
        }

         public async Task<IActionResult> UseFakeData()
        {
            try
            {
                await _driverService.Pause();
                _refreshServiceHolder.SetService("fake");
                _interactionServiceHolder.SetService("fake");
                _passengerService.CleanupFlights();
                await _driverService.Resume();
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
                await _driverService.Pause();
                _refreshServiceHolder.SetService("real");
                _interactionServiceHolder.SetService("real");
                _passengerService.CleanupFlights();
                await _driverService.Resume();
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