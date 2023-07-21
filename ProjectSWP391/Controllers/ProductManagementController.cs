using ProjectSWP391.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;

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
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public ActionResult Details(int id)
        {
            var p = context.Products.Where(n => n.ProductId == id).FirstOrDefault();    
            return View(p);
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpGet]
        public ActionResult CreateProduct(Product product)
        {

            return View();
        }
    }
}
