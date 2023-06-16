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
        public Booking? GetCurrentBooking(int? cusID)
        {
            using (var context = new SWP391Context())
            {
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
            using (var context = new SWP391Context())
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
        public IActionResult ServiceList(int? page)
        {
            // Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            using (var context = new SWP391Context())
            {
                var services = context.Services.OrderBy(s => s.ServiceId);

                // số service hiển thị trên 1 trang
                int pageSize = 8;

                int pageNumber = (page ?? 1);

                // 5. Trả về các Link được phân trang theo kích thước và số trang.
                return View(services.ToPagedList(pageNumber, pageSize));
            }
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