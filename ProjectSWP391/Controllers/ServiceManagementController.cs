using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectSWP391.DAO;
using ProjectSWP391.Models;

namespace ProjectSWP391.Controllers
{
    public class ServiceManagementController : Controller
    {
        private readonly ServiceManagementDAO ServiceDao = new ServiceManagementDAO();
        public IActionResult Index()
        {
            return View(ServiceDao.GetServices());
        }

        public IActionResult Details(int id)
        {
            Service service = ServiceDao.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            //Get category name
            using var context = new SWP391Context();
            ServiceCategory? sg = context.ServiceCategories.Where(s => s.ScategoryId == service.ScategoryId).FirstOrDefault();
            if(sg != null)
            {
                ViewBag.ScategoryName = sg.ScategoryName;
                //Get Related Service in a category
                if (context.Services.Count() - 1 <= 4)
                {
                    List<Service> services = context.Services.Where(s => s.ServiceId != id && s.ScategoryId == sg.ScategoryId).ToList();
                    ViewBag.relateService = services;
                }
                else
                {
                    List<Service> services = context.Services.Where(s => s.ServiceId != id && s.ScategoryId == sg.ScategoryId).Take(4).ToList();
                    ViewBag.relateService = services;
                }
            }
            return View(service);
        }
        //Create
        public ActionResult Create()
        {
            var categories = ServiceDao.GetServiceCategories();
            ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                ServiceDao.AddService(service);
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }

        public IActionResult Edit(int id)
        {
            var service = ServiceDao.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            var categories = ServiceDao.GetServiceCategories();
            ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
            return View(service);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                ServiceDao.EditService(service);
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        public IActionResult Delete(int id)
        {
            var service = ServiceDao.GetServiceById(id);
            ServiceDao.DeleteService(service);
            return RedirectToAction(nameof(Index));
        }
    }
}
