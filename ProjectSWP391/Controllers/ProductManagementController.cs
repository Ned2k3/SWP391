using ProjectSWP391.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEMOSWP391.Controllers
{
    public class ProductManagementController : Controller
    {
        SWP391_V4Context context = new SWP391_V4Context();
        public IActionResult Index()
        {
            var lsProduct = context.Products.ToList(); 
            return View(lsProduct);
        }

        public ActionResult Details(int id)
        {
            var p = context.Products.Where(n => n.ProductId == id).FirstOrDefault();    
            return View(p);
        }

        [HttpGet]
        public ActionResult CreateProduct(Product product)
        {

            return View();
        }
    }
}
