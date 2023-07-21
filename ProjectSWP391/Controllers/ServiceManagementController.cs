using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectSWP391.DAO;
using ProjectSWP391.Models;
using System.Data;
using System.Text.RegularExpressions;
using X.PagedList;

namespace ProjectSWP391.Controllers
{
    public class ServiceManagementController : Controller
    {
        private readonly ServiceManagementDAO ServiceDao = new ServiceManagementDAO();
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Index(string? search, bool isSearch, bool isAscendingPrice = false, int page = 1)
        {
            const int pageSize = 10;
            page = page < 1 ? 1 : page;

            if (!string.IsNullOrEmpty(search))
            {
                var regex = new Regex("\\s{2,}");
                search = regex.Replace(search.Trim(), " ");
            }
            var services = ServiceDao.GetServices(search, isSearch, isAscendingPrice);
            var totalItems = services.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var currentPageItems = services
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["key"] = search;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.IsAscendingPrice = isAscendingPrice; // OrderBy Price for ServiceView
            return View(currentPageItems);
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Details(int id)
        {
            Service service = ServiceDao.GetServiceById(id);
            if (service == null)
            {
                return NotFound();
            }
            //Get category name
            using var context = new SWP391_V4Context();
            ServiceCategory? sg = context.ServiceCategories.Where(s => s.ScategoryId == service.ScategoryId).FirstOrDefault();
            if (sg != null)
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
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Create()
        {
            var categories = ServiceDao.GetServiceCategories();
            ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
            return View();
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Service service)
        {
            service.ServiceName = service.ServiceName?.Trim();
            service.Description = service.Description?.Trim();
            service.Image = service.Image?.Trim();

            if (string.IsNullOrWhiteSpace(service.ServiceName) || service.ServiceName.Length < 5)
            {
                ModelState.AddModelError("ServiceName", "ServiceName must have at least 5 characters.");
            }

            if (service.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            }

            if (service.ScategoryId == null)
            {
                ModelState.AddModelError("ScategoryId", "Category must be choose");
            }

            if (!ModelState.IsValid)
            {
                var categories = ServiceDao.GetServiceCategories();
                ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
                return View("Create");
            }

            Service s = ServiceDao.GetServices("", false, false).FirstOrDefault(s => s.ServiceName.Trim().Equals(service.ServiceName));

            if (s != null)
            {
                ModelState.AddModelError("ServiceName", "ServiceName is already exists!!");
                var categories = ServiceDao.GetServiceCategories();
                ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
                return View("Create");
            }

            ServiceDao.AddService(service);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
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
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Edit(Service service)
        {
            service.ServiceName = service.ServiceName?.Trim();
            service.Description = service.Description?.Trim();
            service.Image = service.Image?.Trim();

            if (string.IsNullOrWhiteSpace(service.ServiceName) || service.ServiceName.Length < 5)
            {
                ModelState.AddModelError("ServiceName", "ServiceName must have at least 5 characters.");
            }

            if (service.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            }

            if (service.ScategoryId == null)
            {
                ModelState.AddModelError("ScategoryId", "Category must be choose");
            }

            if (!ModelState.IsValid)
            {
                var categories = ServiceDao.GetServiceCategories();
                ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
                return View("Create");
            }

            Service s = ServiceDao.GetServices("", false, false).FirstOrDefault(s => s.ServiceName.Trim().Equals(service.ServiceName));

            if (s != null)
            {
                ModelState.AddModelError("ServiceName", "ServiceName is already exists!!");
                var categories = ServiceDao.GetServiceCategories();
                ViewBag.ScategoryId = new SelectList(categories, "ScategoryId", "ScategoryName");
                return View("Edit");
            }
            ServiceDao.EditService(service);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Delete(int id)
        {
            Console.WriteLine(id);
            var service = ServiceDao.GetServiceById(id);
            ServiceDao.DeleteService(service);
            return RedirectToAction(nameof(Index));
        }
    }
}
