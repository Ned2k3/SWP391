using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;
using System.Diagnostics;
using X.PagedList;

namespace ProjectSWP391.Controllers
{
    public class CustomerManagementController : Controller
    {
        private readonly string[] _PriceFilter = { "< 20$", "20$ - 50$", "51$ - 75$", "> 75$" };
        public Booking? GetCurrentBooking(int? cusID)
        {
            using (var context = new SWP391_V4Context())
            {
                //Customer ID can not equal to 0
                if (cusID == 0) return null;
                Booking? bk = context.Bookings.Where(b => ((b.BookingDate.Date == DateTime.Today.Date && b.Shift > DateTime.Now.Hour)
                || (b.BookingDate.Date > DateTime.Today.Date)) && b.CustomerId == cusID).FirstOrDefault();

                if (bk != null)
                {
                    return bk;
                }
                return null;
            }
        }

        public IActionResult LandingPage(int? booked)
        {
            using (var context = new SWP391_V4Context())
            {
                List<Product> products = context.Products.OrderByDescending(p => p.Quantity).Take(8).ToList();
                List<Service> services = context.Services.OrderBy(s => s.Price).Take(6).ToList();
                ViewBag.ServiceList = services;

                //Check if user has book or not
                Account? acc = Global.CurrentUser;
                int? id = acc == null ? 0 : acc.AccountId;
                Booking? bk = GetCurrentBooking(id);
                if (bk != null)
                {
                    ViewBag.currentBooking = bk;
                }
                else
                {
                    //if guest enter booked phone number then show message
                    if (booked != null)
                    {
                        Booking? booking = context.Bookings.Where(b => b.BookingId == booked).FirstOrDefault();
                        if (booking != null)
                        {
                            ViewBag.currentBooking = booking;
                        }
                    }
                }
                return View(products);
            }
        }

        public IActionResult ProductList(int? page, string? sn, string? sc, int sp)
        {
            // Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            using (var context = new SWP391_V4Context())
            {
                if (sn == null) sn = String.Empty;
                ViewBag.searchName = sn;
                if (sc == null) sc = String.Empty;
                //filter by name and category
                var products = (from product in context.Products
                                join pcategory in context.ProductCategories
                                on product.PcategoryId equals pcategory.PcategoryId
                                where product.ProductName.Contains(sn) && pcategory.PcategoryName.Contains(sc)
                                select product).OrderBy(s => s.PcategoryId);
                //Get all service category
                var categories = context.ProductCategories.ToList();
                ViewBag.categoryFilter = categories;
                //Save the previous searchCategory
                ViewBag.searchCategory = sc;
                //Get Price filter list
                ViewBag.priceFilter = _PriceFilter;
                //Save the previuos searchPrice
                ViewBag.searchPrice = sp;
                // số service hiển thị trên 1 trang
                int pageSize = 8;

                int pageNumber = (page ?? 1);

                //filter by price
                switch (sp)
                {
                    case 1:
                        {
                            var filterProducts = products.Where(s => s.Price < 20).OrderBy(p => p.Price).ToList();
                            return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                        }
                    case 2:
                        {
                            var filterProducts = products.Where(s => s.Price >= 20 && s.Price <= 50).OrderBy(p => p.Price).ToList();
                            return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                        }
                    case 3:
                        {
                            var filterProducts = products.Where(s => s.Price > 50 && s.Price <= 75).OrderBy(p => p.Price).ToList();
                            return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                        }
                    case 4:
                        {
                            var filterProducts = products.Where(s => s.Price > 75).OrderBy(p => p.Price).ToList();
                            return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                        }
                    default:
                        {
                            return View("~/Views/CustomerManagement/ProductList.cshtml", products.ToPagedList(pageNumber, pageSize));
                        }
                }
            }
        }

        [HttpPost]
        public IActionResult FilterProduct()
        {
            string searchName = Request.Form["searchName"];
            string searchCategory = Request.Form["searchCategory"];
            int searchPrice = Convert.ToInt32(Request.Form["searchPrice"]);
            return ProductList(1, searchName, searchCategory, searchPrice);
        }

        public IActionResult ServiceList(int? page, string? sn, string? sc, int sp)
        {
            // Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            using (var context = new SWP391_V4Context())
            {
                if (sn == null) sn = String.Empty;
                ViewBag.searchName = sn;
                if (sc == null) sc = String.Empty;
                //filter by name and category
                var services = (from service in context.Services join scategory in context.ServiceCategories
                                on service.ScategoryId equals scategory.ScategoryId
                                where service.ServiceName.Contains(sn) && scategory.ScategoryName.Contains(sc)
                                select service).OrderBy(s => s.ServiceId);
                //Get all service category
                var categories = context.ServiceCategories.ToList();
                ViewBag.categoryFilter = categories;
                //Save the previous searchCategory
                ViewBag.searchCategory = sc;
                //Get Price filter list
                ViewBag.priceFilter = _PriceFilter;
                //Save the previuos searchPrice
                ViewBag.searchPrice = sp;
                // số service hiển thị trên 1 trang
                int pageSize = 6;

                int pageNumber = (page ?? 1);

                //filter by price
                switch (sp)
                {
                    case 1:
                        {
                            var filterServices = services.Where(s => s.Price < 20).OrderBy(s => s.Price).ToList();
                            return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                        }
                    case 2:
                        {
                            var filterServices = services.Where(s => s.Price >= 20 && s.Price <= 50).OrderBy(s => s.Price).ToList();
                            return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                        }
                    case 3:
                        {
                            var filterServices = services.Where(s => s.Price > 50 && s.Price <= 75).OrderBy(s => s.Price).ToList();
                            return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                        }
                    case 4:
                        {
                            var filterServices = services.Where(s => s.Price > 75).OrderBy(s => s.Price).ToList();
                            return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                        }
                    default:
                        {
                            return View("~/Views/CustomerManagement/ServiceList.cshtml", services.ToPagedList(pageNumber, pageSize));
                        }
                }
            }
        }

        [HttpPost]
        public IActionResult FilterService()
        {
            string searchName = Request.Form["searchName"];
            string searchCategory = Request.Form["searchCategory"];
            int searchPrice = Convert.ToInt32(Request.Form["searchPrice"]);
            return ServiceList(1,searchName,searchCategory, searchPrice);
        }

        public IActionResult ServiceDetail(int? sId)
        {
            using var context = new SWP391_V4Context();
            Service? service = context.Services.Where(s => s.ServiceId == sId).FirstOrDefault();
            if (service == null)
            {
                return NotFound();
            }
            //Get category name
            ServiceCategory? sg = context.ServiceCategories.Where(s => s.ScategoryId == service.ScategoryId).FirstOrDefault();
            if (sg != null)
            {
                ViewBag.ScategoryName = sg.ScategoryName;
                //Get Related Service in a category
                if (context.Services.Count() - 1 <= 4)
                {
                    List<Service> services = context.Services.Where(s => s.ServiceId != sId && s.ScategoryId == sg.ScategoryId).ToList();
                    ViewBag.relateService = services;
                }
                else
                {
                    List<Service> services = context.Services.Where(s => s.ServiceId != sId && s.ScategoryId == sg.ScategoryId).Take(4).ToList();
                    ViewBag.relateService = services;
                }
            }
            return View(service);
        }
        public IActionResult BlogList()
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