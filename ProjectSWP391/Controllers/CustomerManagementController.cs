using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using System.Diagnostics;
using X.PagedList;

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

        public IActionResult ProductList(int? page)
        {
            // Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            using (var context = new SWP391Context())
            {
                var products = context.Products.OrderBy(p => p.ProductId);

                // số product hiển thị trên 1 trang
                int pageSize = 8;

                int pageNumber = (page ?? 1);

                // 5. Trả về các Link được phân trang theo kích thước và số trang.
                return View(products.ToPagedList(pageNumber, pageSize));
            }
        }
        public IActionResult ServiceList()
        {
            List<Service> services;
            using (var context = new SWP391Context())
            {
                services = context.Services.ToList();
            }
            return View(services);
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