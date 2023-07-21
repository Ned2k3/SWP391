using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectSWP391.DAO;
using ProjectSWP391.Models;
using System.Data;
using System.Globalization;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;

namespace ProjectSWP391.Controllers
{
    public class BlogManagementController : Controller
    {
        private readonly BlogManagementDAO dao = new BlogManagementDAO();
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Index(string? search, bool isSearch, bool isAscending = false, int page = 1)
        {

            if (!string.IsNullOrEmpty(search))
            {
                var regex = new Regex("\\s{2,}");
                search = regex.Replace(search.Trim(), " ");
            }
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
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
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
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Create()
        {
            var accounts = dao.GetAccounts();
            ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
            return View();
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Blog blog)
        {
            blog.Title = blog.Title?.Trim();
            blog.Content = blog.Content?.Trim();

            if (string.IsNullOrWhiteSpace(blog.Title) || blog.Title.Length < 5)
            {
                ModelState.AddModelError("Title", "Title must have at least 5 characters.");
            }
            if (string.IsNullOrWhiteSpace(blog.Content) || blog.Title.Length < 10)
            {
                ModelState.AddModelError("Title", "Title must have at least 5 characters.");
            }
            if (!ModelState.IsValid)
            {
                var accounts = dao.GetAccounts();
                ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                return View("Create");
            }
            Blog b = dao.GetBlogs("", false, false).FirstOrDefault(b => b.Title.Trim().Equals(blog.Title));

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
            blog.BlogDate = DateTime.Now;
            dao.AddBlog(blog);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
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
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Edit(Blog blog)
        {
            blog.Title = blog.Title?.Trim();
            blog.Content = blog.Content?.Trim();

            if (string.IsNullOrWhiteSpace(blog.Title) || blog.Title.Length < 5)
            {
                ModelState.AddModelError("Title", "Title must have at least 5 characters.");
            }
            if (string.IsNullOrWhiteSpace(blog.Content) || blog.Title.Length < 10)
            {
                ModelState.AddModelError("Title", "Title must have at least 5 characters.");
            }

            if (!ModelState.IsValid)
            {
                var accounts = dao.GetAccounts();
                ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                return View("Edit");
            }


            var b = dao.GetBlogs("", false, false).FirstOrDefault(b => b.Title.Trim().Equals(blog.Title));
            if (b != null)
            {
                ModelState.AddModelError("Title", "Title is already exists!!");
                var accounts = dao.GetAccounts();
                ViewBag.AccountId = new SelectList(accounts, "AccountId", "FullName");
                return View(blog);
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
            blog.BlogDate = DateTime.Now;
            dao.EditBlog(blog);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Delete(int id)
        {
            Console.WriteLine(id);
            var blog = dao.GetBlogById(id);
            dao.DeleteBlog(blog);
            return RedirectToAction(nameof(Index));
        }
    }
}
