using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectSWP391.DAO;
using ProjectSWP391.Models;
using System.Globalization;
using System.Reflection.Metadata;

namespace ProjectSWP391.Controllers
{
    public class BlogManagementController : Controller
    {
        private readonly BlogManagementDAO dao = new BlogManagementDAO();
        public IActionResult Index(string? search, bool isSearch, bool isAscending = false, int page = 1)
        {
            var blogs = dao.GetBlogs(search, isSearch, isAscending);

            //Paging:
            const int pageSize = 10;
            page = page < 1 ? 1 : page;

            var totalItems = blogs.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var currentPageItems = blogs
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["key"] = search;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.IsAscending = isAscending; // OrderBy Date for BlogView
            return View(currentPageItems);
        }

        public IActionResult Detail(int id)
        {
            var blog = dao.GetBlogById(id);
            if (blog == null)
            {
                return NotFound();
            }

            var accounts = dao.GetAccounts();

            string email = "";
            string fullName = "";
            int? role = 1;
            foreach (var account in accounts)
            {
                if (account.AccountId == blog.AccountId)
                {
                    email = account.Email;
                    fullName = account.FullName;
                    role = account.Role ?? 1;
                }
            }
            ViewBag.Email = email;
            ViewBag.FullName = fullName;
            ViewBag.Role = role;
            return View(blog);
        }

        public IActionResult Create()
        {
            var accounts = dao.GetAccounts();
            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Blog blog)
        {

            Blog b = dao.GetBlogs("", false, false).FirstOrDefault(b => b.Title == blog.Title);

            if (b != null)
            {
                ModelState.AddModelError("Title", "Title is already exists!!");
                var accounts = dao.GetAccounts();
                ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                return View("Create");
            }

            /*            DateTime tempDate;
                        if (!DateTime.TryParseExact(blog.BlogDate?.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            ModelState.AddModelError("BlogDate", "Invalid date format!");
                            var accounts = dao.GetAccounts();
                            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                            return View("Create");
                        }
                        else if (tempDate < new DateTime(2000, 12, 31) || tempDate > DateTime.Now)
                        {
                            ModelState.AddModelError("BlogDate", "Date must be between 31/12/2000 and today!");
                            var accounts = dao.GetAccounts();
                            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                            return View("Create");
                        }*/

            if (!ModelState.IsValid)
            {
                var accounts = dao.GetAccounts();
                ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                return View("Create");
            }
            dao.AddBlog(blog);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var blog = dao.GetBlogById(id);
            if (blog == null)
            {
                return NotFound();
            }

            var accounts = dao.GetAccounts();
            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
            return View(blog);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Blog blog)
        {
            var oldBlog = dao.GetBlogById(blog.BlogId);
            if (oldBlog == null)
            {
                return NotFound();
            }

            if (blog.Title != oldBlog.Title)
            {
                // Nếu người dùng thay đổi ServiceName, kiểm tra xem ServiceName mới có trùng với ServiceName của các ID khác không
                var b = dao.GetBlogs("", false, false).FirstOrDefault(b => b.Title == blog.Title && b.BlogId != blog.BlogId);
                if (b != null)
                {
                    ModelState.AddModelError("Title", "Title is already exists!!");
                    var accounts = dao.GetAccounts();
                    ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                    return View(blog);
                }
            }

            /*            DateTime tempDate;
                        if (!DateTime.TryParseExact(blog.BlogDate?.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out tempDate))
                        {
                            ModelState.AddModelError("BlogDate", "Invalid date format!");
                            var accounts = dao.GetAccounts();
                            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                            return View("Edit");
                        }
                        else if (tempDate < new DateTime(2000, 12, 31) || tempDate > DateTime.Now)
                        {
                            ModelState.AddModelError("BlogDate", "Date must be between 31/12/2000 and today!");
                            var accounts = dao.GetAccounts();
                            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                            return View("Edit");
                        }*/

            if (!ModelState.IsValid)
            {
                var accounts = dao.GetAccounts();
                ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                return View("Edit");
            }
            dao.EditBlog(blog);
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            Console.WriteLine(id);
            var blog = dao.GetBlogById(id);
            dao.DeleteBlog(blog);
            return RedirectToAction(nameof(Index));
        }
    }
}
