using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;
using System.Diagnostics;

namespace ProjectSWP391.Controllers
{
    public class CustomerManagementController : Controller
    {
        private readonly ILogger<CustomerManagementController> _logger;

        public CustomerManagementController(ILogger<CustomerManagementController> logger)
        {
            _logger = logger;
        }

        public IActionResult LandingPage()
        {

            return View("~/Views/CustomerManagement/LandingPage.cshtml");
        }

        public IActionResult ProductList()
        {
            return View();
        }
        public IActionResult ServiceList()
        {
            return View();
        }
        public IActionResult BlogList()
        {
            return View();
        }

        public IActionResult BookingService(int serviceID)
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}