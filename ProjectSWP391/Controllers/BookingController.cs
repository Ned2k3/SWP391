using Microsoft.AspNetCore.Mvc;

namespace ProjectSWP391.Controllers
{
    public class BookingController : Controller
    {
        public IActionResult Load()
        {
            if (HttpContext.Session.GetString("userID") == null) ViewData["isGuest"] = true;
            return View();
        }
    }
}
