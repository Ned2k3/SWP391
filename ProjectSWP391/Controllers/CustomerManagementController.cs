using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;
using System.Diagnostics;
using System.Reflection;
using X.PagedList;

namespace ProjectSWP391.Controllers
{
    public class CustomerManagementController : Controller
    {
        private readonly string[] _PriceFilter = { "< 20$", "20$ - 50$", "51$ - 75$", "> 75$" };

        private readonly IWebHostEnvironment _webHostEnvironment;

        public CustomerManagementController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public Booking? GetCurrentBooking(int? cusID)
        {
            //Customer ID can not equal to 0
            if (cusID == 0) return null;
            Booking? bk = context.Bookings.Where(b => ((b.BookingDate.Date == DateTime.Today.Date && b.Shift > DateTime.Now.Hour)
            || (b.BookingDate.Date > DateTime.Today.Date)) && b.CustomerId == cusID).FirstOrDefault();

            if (bk != null)
            {
                return bk;
            }
            return null;
        }

        public IActionResult LandingPage(int? booked, string[]? message)
        {
            if(message != null)
            {
                ViewBag.message = message;
            }
            List<Product> products = context.Products.OrderByDescending(p => p.Quantity).Take(8).ToList();
            List<Service> services = context.Services.OrderBy(s => s.Price).Take(6).ToList();
            var blogs = context.Blogs.OrderByDescending(b => b.BlogDate).Take(4).Select(b => new
            {
                ID = b.BlogId,
                Title = b.Title,
                Content = b.Content,
                Date = b.BlogDate,
                Author = b.Account.FullName
            }).ToList();
            ViewBag.BlogList = blogs;
            ViewBag.ServiceList = services;

            //Check if user has book or not
            Account? acc = Global.CurrentUser;
            int? id = acc == null ? 0 : acc.AccountId;
            Booking? bk = GetCurrentBooking(id);
            if (bk != null)
            {
                ViewBag.currentBooking = bk;
            }
            else
            {
                //if guest enter booked phone number then show message
                if (booked != null)
                {
                    Booking? booking = context.Bookings.Where(b => b.BookingId == booked).FirstOrDefault();
                    if (booking != null)
                    {
                        ViewBag.currentBooking = booking;
                    }
                }
            }
            return View(products);
        }

        public IActionResult ProductList(int? page, string? sn, string? sc, int sp)
        {
            // Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            if (sn == null) sn = String.Empty;
            ViewBag.searchName = sn;
            if (sc == null) sc = String.Empty;
            //filter by name and category
            var products = (from product in context.Products
                            join pcategory in context.ProductCategories
                            on product.PcategoryId equals pcategory.PcategoryId
                            where product.ProductName.Contains(sn) && pcategory.PcategoryName.Contains(sc) && product.IsActive == true
                            select product).OrderBy(p => p.ProductId);
            //Get all service category
            var categories = context.ProductCategories.ToList();
            ViewBag.categoryFilter = categories;
            //Save the previous searchCategory
            ViewBag.searchCategory = sc;
            //Get Price filter list
            ViewBag.priceFilter = _PriceFilter;
            //Save the previuos searchPrice
            ViewBag.searchPrice = sp;
            // số service hiển thị trên 1 trang
            int pageSize = 8;

            int pageNumber = (page ?? 1);

            //filter by price
            switch (sp)
            {
                case 1:
                    {
                        var filterProducts = products.Where(s => s.Price < 20).OrderBy(p => p.Price).ToList();
                        return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                    }
                case 2:
                    {
                        var filterProducts = products.Where(s => s.Price >= 20 && s.Price <= 50).OrderBy(p => p.Price).ToList();
                        return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                    }
                case 3:
                    {
                        var filterProducts = products.Where(s => s.Price > 50 && s.Price <= 75).OrderBy(p => p.Price).ToList();
                        return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                    }
                case 4:
                    {
                        var filterProducts = products.Where(s => s.Price > 75).OrderBy(p => p.Price).ToList();
                        return View("~/Views/CustomerManagement/ProductList.cshtml", filterProducts.ToPagedList(pageNumber, pageSize));
                    }
                default:
                    {
                        return View("~/Views/CustomerManagement/ProductList.cshtml", products.ToPagedList(pageNumber, pageSize));
                    }
            }
        }

        [HttpPost]
        public IActionResult FilterProduct()
        {
            string searchName = Request.Form["searchName"];
            string searchCategory = Request.Form["searchCategory"];
            int searchPrice = Convert.ToInt32(Request.Form["searchPrice"]);
            return ProductList(1, searchName, searchCategory, searchPrice);
        }

        public IActionResult ServiceList(int? page, string? sn, string? sc, int sp)
        {
            // Nếu page = null thì đặt lại là 1.
            if (page == null) page = 1;

            if (sn == null) sn = String.Empty;
            ViewBag.searchName = sn;
            if (sc == null) sc = String.Empty;
            //filter by name and category
            var services = (from service in context.Services
                            join scategory in context.ServiceCategories
                            on service.ScategoryId equals scategory.ScategoryId
                            where service.ServiceName.Contains(sn) && scategory.ScategoryName.Contains(sc) && service.IsActive == true
                            select service).OrderBy(s => s.ServiceId);
            //Get all service category
            var categories = context.ServiceCategories.ToList();
            ViewBag.categoryFilter = categories;
            //Save the previous searchCategory
            ViewBag.searchCategory = sc;
            //Get Price filter list
            ViewBag.priceFilter = _PriceFilter;
            //Save the previuos searchPrice
            ViewBag.searchPrice = sp;
            // số service hiển thị trên 1 trang
            int pageSize = 6;

            int pageNumber = (page ?? 1);

            //filter by price
            switch (sp)
            {
                case 1:
                    {
                        var filterServices = services.Where(s => s.Price < 20).OrderBy(s => s.Price).ToList();
                        return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                    }
                case 2:
                    {
                        var filterServices = services.Where(s => s.Price >= 20 && s.Price <= 50).OrderBy(s => s.Price).ToList();
                        return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                    }
                case 3:
                    {
                        var filterServices = services.Where(s => s.Price > 50 && s.Price <= 75).OrderBy(s => s.Price).ToList();
                        return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                    }
                case 4:
                    {
                        var filterServices = services.Where(s => s.Price > 75).OrderBy(s => s.Price).ToList();
                        return View("~/Views/CustomerManagement/ServiceList.cshtml", filterServices.ToPagedList(pageNumber, pageSize));
                    }
                default:
                    {
                        return View("~/Views/CustomerManagement/ServiceList.cshtml", services.ToPagedList(pageNumber, pageSize));
                    }
            }
        }

        [HttpPost]
        public IActionResult FilterService()
        {
            string searchName = Request.Form["searchName"];
            string searchCategory = Request.Form["searchCategory"];
            int searchPrice = Convert.ToInt32(Request.Form["searchPrice"]);
            return ServiceList(1, searchName, searchCategory, searchPrice);
        }

        private readonly SWP391_V4Context context = new SWP391_V4Context();
        public IActionResult ServiceDetail(int? sId, int pageFeedback = 1)
        {
            Service? service = context.Services.Where(s => s.ServiceId == sId).FirstOrDefault();
            if (service == null)
            {
                return NotFound();
            }
            //Get category name
            ServiceCategory? sg = context.ServiceCategories.Where(s => s.ScategoryId == service.ScategoryId).FirstOrDefault();
            if (sg != null)
            {
                ViewBag.ScategoryName = sg.ScategoryName;
                //Get Related Service in a category
                if (context.Services.Count() - 1 <= 4)
                {
                    List<Service> services = context.Services.Where(s => s.ServiceId != sId && s.ScategoryId == sg.ScategoryId).ToList();
                    ViewBag.relateService = services;
                }
                else
                {
                    List<Service> services = context.Services.Where(s => s.ServiceId != sId && s.ScategoryId == sg.ScategoryId).Take(4).ToList();
                    ViewBag.relateService = services;
                }
            }

            const int pageFeedbackSize = 6;
            var feedbacks = context.Feedbacks.Where(f => f.ServiceId == sId).OrderByDescending(f => f.Date).ToList();
            if (feedbacks.Count > 0)
            {
                var accounts = context.Accounts.ToList();

                // paging
                int totalItems = feedbacks.Count;
                int totalPages = (int)Math.Ceiling(totalItems / (double)pageFeedbackSize);
                int skip = (pageFeedback - 1) * pageFeedbackSize;
                feedbacks = feedbacks.Skip(skip).Take(pageFeedbackSize).ToList();

                ViewBag.Accounts = accounts;
                ViewBag.Feedbacks = feedbacks;
                ViewBag.TotalPages = totalPages;
                ViewBag.CurrentPage = pageFeedback;
            }
            return View(service);
        }

        [HttpPost]
        public IActionResult PostServiceFeedback(int accountId, int serviceId, string content, DateTime dateTimeNow)
        {
            try
            {
                bool hasFeedback = context.Feedbacks.Any(f => f.AccountId == accountId && f.ServiceId == serviceId);

                if (hasFeedback)
                {
                    ModelState.AddModelError("FeedbackError", "You have already given feedback for this product.");
                    return RedirectToAction("ServiceDetail", new { sId = serviceId });
                }

                var feedback = new Feedback
                {
                    ProductId = null,
                    AccountId = accountId,
                    ServiceId = serviceId,
                    Content = content,
                    Date = dateTimeNow
                };
                context.Feedbacks.Add(feedback);
                context.SaveChanges();

                return RedirectToAction("ServiceDetail", new { sId = serviceId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public IActionResult EditServiceFeedback(int accountId, int serviceId, int feedbackId, string content, DateTime dateTimeNow)
        {
            try
            {
                var feedback = new Feedback
                {
                    FeedbackId = feedbackId,
                    ProductId = null,
                    AccountId = accountId,
                    ServiceId = serviceId,
                    Content = content,
                    Date = dateTimeNow
                };
                context.Feedbacks.Update(feedback);
                context.SaveChanges();
                return RedirectToAction("ServiceDetail", new { sId = serviceId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public IActionResult DeleteServiceFeedback(int feedbackId, int serviceId)
        {
            try
            {
                var feedback = context.Feedbacks.SingleOrDefault(f => f.FeedbackId == feedbackId);
                context.Feedbacks.Remove(feedback);
                context.SaveChanges();
                return RedirectToAction("ServiceDetail", new { sId = serviceId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(errorMessage);
            }
        }

        public IActionResult ProductDetails(int id, int pageFeedback = 1)
        {
            Product product = null;
            try
            {
                product = context.Products.SingleOrDefault(p => p.ProductId == id);
                if (product == null)
                {
                    return NotFound();
                }
                //Get category name
                ProductCategory? pg = context.ProductCategories.Where(p => p.PcategoryId == product.PcategoryId).FirstOrDefault();
                if (pg != null)
                {
                    ViewBag.PcategoryName = pg.PcategoryName;
                    //Get Related Products in a category
                    if (context.Products.Count() - 1 <= 4)
                    {
                        List<Product> products = context.Products.Where(p => p.ProductId != id && p.PcategoryId == pg.PcategoryId).ToList();
                        ViewBag.relateProduct = products;
                    }
                    else
                    {
                        List<Product> products = context.Products.Where(p => p.ProductId != id && p.PcategoryId == pg.PcategoryId).Take(4).ToList();
                        ViewBag.relateProduct = products;
                    }
                }

                const int pageFeedbackSize = 6;
                var feedbacks = context.Feedbacks.Where(f => f.ProductId == id).OrderByDescending(f => f.Date).ToList();
                if (feedbacks.Count > 0)
                {
                    var accounts = context.Accounts.ToList();

                    // paging
                    int totalItems = feedbacks.Count;
                    int totalPages = (int)Math.Ceiling(totalItems / (double)pageFeedbackSize);
                    int skip = (pageFeedback - 1) * pageFeedbackSize;
                    feedbacks = feedbacks.Skip(skip).Take(pageFeedbackSize).ToList();

                    ViewBag.Accounts = accounts;
                    ViewBag.Feedbacks = feedbacks;
                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentPage = pageFeedback;
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult PostFeedback(int accountId, int productId, string content, DateTime dateTimeNow)
        {
            try
            {
                bool hasFeedback = context.Feedbacks.Any(f => f.AccountId == accountId && f.ProductId == productId);

                if (hasFeedback)
                {
                    ModelState.AddModelError("FeedbackError", "You have already given feedback for this product.");
                    return RedirectToAction("ProductDetails", new { id = productId });
                }

                var feedback = new Feedback
                {
                    ProductId = productId,
                    AccountId = accountId,
                    ServiceId = 1,
                    Content = content,
                    Date = dateTimeNow
                };
                context.Feedbacks.Add(feedback);
                context.SaveChanges();

                return RedirectToAction("ProductDetails", new { id = productId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public IActionResult EditFeedback(int accountId, int productId, int feedbackId, string content, DateTime dateTimeNow)
        {
            try
            {
                var feedback = new Feedback
                {
                    FeedbackId = feedbackId,
                    ProductId = productId,
                    AccountId = accountId,
                    ServiceId = 1,
                    Content = content,
                    Date = dateTimeNow
                };
                context.Feedbacks.Update(feedback);
                context.SaveChanges();
                return RedirectToAction("ProductDetails", new { id = productId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(errorMessage);
            }
        }

        [HttpPost]
        public IActionResult DeleteFeedback(int feedbackId, int productId)
        {
            try
            {
                var feedback = context.Feedbacks.SingleOrDefault(f => f.FeedbackId == feedbackId);
                context.Feedbacks.Remove(feedback);
                context.SaveChanges();
                return RedirectToAction("ProductDetails", new { id = productId });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(errorMessage);
            }
        }
        public IActionResult BlogList(int? page, string sName, bool myBlog, int sort)
        {
            if (page == null) page = 1;
            int pageSize = 4;
            int pageNumber = (page ?? 1);

            ViewBag.isMyBlog = myBlog;
            ViewBag.sort = sort;
            sName = (sName == null ? "" : sName);
            ViewBag.searchName = sName;

            //if user view blog list with authenticated account then show thier own blogs
            if (myBlog)
            {
                //if sort by lastest blogs
                if (sort == 1)
                {
                    var myblogs = context.Blogs.Where(b => b.AccountId == Global.CurrentUser.AccountId && (b.Title.Contains(sName) || b.Account.Email.Contains(sName))).OrderByDescending(b => b.BlogId).Select(b => new
                    {
                        ID = b.BlogId,
                        Title = b.Title,
                        Content = b.Content,
                        Author = b.Account,
                        Date = b.BlogDate
                    }).ToList();
                    return View(myblogs.ToPagedList(pageNumber, pageSize));
                }
                //if sort by oldest blogs
                else if (sort == 2)
                {
                    var myblogs = context.Blogs.Where(b => b.AccountId == Global.CurrentUser.AccountId && (b.Title.Contains(sName) || b.Account.Email.Contains(sName))).OrderBy(b => b.BlogId).Select(b => new
                    {
                        ID = b.BlogId,
                        Title = b.Title,
                        Content = b.Content,
                        Author = b.Account,
                        Date = b.BlogDate
                    }).ToList();
                    return View(myblogs.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var myblogs = context.Blogs.Where(b => b.AccountId == Global.CurrentUser.AccountId && (b.Title.Contains(sName) || b.Account.Email.Contains(sName))).OrderBy(b => b.Title).Select(b => new
                    {
                        ID = b.BlogId,
                        Title = b.Title,
                        Content = b.Content,
                        Author = b.Account,
                        Date = b.BlogDate
                    }).ToList();
                    return View(myblogs.ToPagedList(pageNumber, pageSize));
                }
            }
            //else show all blogs
            else
            {
                //if sort by lastest blogs
                if (sort == 1)
                {
                    var myblogs = context.Blogs.Where(b => b.Title.Contains(sName) || b.Account.Email.Contains(sName)).OrderByDescending(b => b.BlogId).Select(b => new
                    {
                        ID = b.BlogId,
                        Title = b.Title,
                        Content = b.Content,
                        Author = b.Account,
                        Date = b.BlogDate
                    }).ToList();
                    return View(myblogs.ToPagedList(pageNumber, pageSize));
                }
                //if sort by oldest blogs
                else if (sort == 2)
                {
                    var myblogs = context.Blogs.Where(b => b.Title.Contains(sName) || b.Account.Email.Contains(sName)).OrderBy(b => b.BlogId).Select(b => new
                    {
                        ID = b.BlogId,
                        Title = b.Title,
                        Content = b.Content,
                        Author = b.Account,
                        Date = b.BlogDate
                    }).ToList();
                    return View(myblogs.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    var blogs = context.Blogs.Where(b => b.Title.Contains(sName) || b.Account.Email.Contains(sName)).OrderBy(b => b.Title).Select(b => new
                    {
                        ID = b.BlogId,
                        Title = b.Title,
                        Content = b.Content,
                        Author = b.Account,
                        Date = b.BlogDate
                    });
                    return View(blogs.ToPagedList(pageNumber, pageSize));
                }
            }
        }

        public IActionResult CreateBlog(int blogId)
        {
            //Find blog to edit
            Blog? blog = context.Blogs.FirstOrDefault(b => b.BlogId == blogId);
            return View(blog);
        }

        [HttpPost]
        public JsonResult UploadImage(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Generate a unique filename for the image
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Set the physical path to save the image
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                var path = Path.Combine(uploadsFolder, filename);

                // Create the "Uploads" directory if it doesn't exist
                Directory.CreateDirectory(uploadsFolder);

                // Save the file to the server
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Return the URL of the saved image to TinyMCE
                var imageUrl = Url.Content("~/Uploads/" + filename);
                return Json(new { location = imageUrl });
            }

            // If the image upload fails, return an error
            return Json(new { error = "Image upload failed." });
        }

        [HttpPost]
        public IActionResult CreateBlog(Blog blog)
        {
            string title = blog.Title.Trim();
            blog.Title = title;
            blog.BlogDate = DateTime.Now;
            blog.AccountId = Global.CurrentUser.AccountId;
            context.Blogs.Add(blog);
            context.SaveChanges();
            return RedirectToAction("BlogList", new { sName = String.Empty, myBlog = true, sort = 0 });
        }

        public IActionResult BlogDetail(int id)
        {
            Blog? blog = context.Blogs.FirstOrDefault(b => b.BlogId == id);
            if (blog != null)
            {
                Account? author = context.Accounts.FirstOrDefault(a => a.AccountId == blog.AccountId);
                blog.Account = author;
                return View(blog);
            }
            return RedirectToAction("BlogList");
        }

        [HttpPost]
        public IActionResult SearchBlog(string searchName, bool isMyBlog, int sortDate)
        {
            return RedirectToAction("BlogList", new { sName = searchName, myBlog = isMyBlog, sort = sortDate });
        }

        public IActionResult DeleteBlog(int id)
        {
            Blog? blog = context.Blogs.FirstOrDefault(b => b.BlogId == id);
            if (blog != null)
            {
                context.Remove(blog);
                context.SaveChanges();
            }
            string searchName = string.Empty;
            int sortDate = 0;
            bool isMyBlog = true;
            return RedirectToAction("BlogList", new { sName = searchName, myBlog = isMyBlog, sort = sortDate });
        }

        public IActionResult EditBlog(int id)
        {
            return RedirectToAction("CreateBlog", new { blogID = id });
        }

        [HttpPost]
        public IActionResult EditBlog(Blog blog)
        {
            Blog? b = context.Blogs.FirstOrDefault(b => b.BlogId == blog.BlogId);
            if (b != null)
            {
                b.Title = blog.Title.Trim();
                b.Content = blog.Content;
                b.BlogDate = DateTime.Now;
                context.SaveChanges();
            }
            return RedirectToAction("BlogList", new { sName = String.Empty, myBlog = true, sort = 0 });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}