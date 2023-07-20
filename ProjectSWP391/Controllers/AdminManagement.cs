using Microsoft.AspNetCore.Mvc;

namespace ProjectSWP391.Controllers
{
    public class AdminManagement : Controller
    {
        public IActionResult DashBoard()
        {
            return View();
        }
    }
}
