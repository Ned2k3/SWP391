using Microsoft.AspNetCore.Mvc;

namespace ProjectSWP391.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Http401()
        {
            return View();
        }
        public IActionResult Http403()
        {
            return View();
        }
        public IActionResult CommonError()
        {
            return View();
        }
        public IActionResult TimeCounter()
        {
            return View();
        }
    }
}
