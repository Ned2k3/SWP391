using Microsoft.AspNetCore.Mvc;

namespace ProjectSWP391.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
