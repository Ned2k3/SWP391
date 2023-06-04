using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;


namespace ProjectSWP391.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Load()
        {
            ViewData["Title"] = "Booking service";
            ViewBag.Customer = Global.CurrentUser;
            List<Account> accounts;
            using(var context = new SWP391Context())
            {
                accounts = context.Accounts.Where(i => i.Role == 2).ToList();
            }
            return View();
        }
    }
}
