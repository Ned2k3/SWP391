using DEMOSWP391.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEMOSWP391.Controllers
{
    public class ProductController : Controller
    {
        swp391Context context = new swp391Context();
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
