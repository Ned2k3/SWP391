using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using System.Linq;

namespace ProjectSWP391.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SWP391Context context;
        public AuthenticationController(SWP391Context _context)
        {
            context = _context;
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(Account a)
        {
            if (a == null)
            {
                //if user doesnt Enter tai khoan
                ViewBag.ErrorMsg = "User must enter an Email and Pasword!";
                return View();
            }
            var account = context.Accounts.Where(b => b.Email == a.Email && b.Password == a.Password).SingleOrDefault();
            //var account = context.Accounts.ToList();
            if (account == null)
            {
                //message Login error
                ViewBag.ErrorMsg = "Account does not exist!";
                return View();
            }
            else
            {
                HttpContext.Session.SetString("userID", a.AccountId.ToString());
                //message
                if (account.Role == 1)
                {
                    return View("~/Views/AdminManagement/ADashBoard.cshtml");
                }
                else if (account.Role == 2)
                {
                    return View("~/Views/EmployeeManagement/EDashBoard.cshtml");
                }
                else
                {
                    return View("~/Views/CustomerManagement/LandingPage.cshtml");
                }
                //return View();
            }

        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string email, string password)
        {

            if (context.Accounts.Any(x => x.Email == email))
            {
                ViewBag.ErrorMsg = "Account has existed";
                return View();
            }
            else
            {
                Account c = new Account();
                c.Password = password;
                c.Email = email;
                c.IsActive = true;

                context.Add(c);
                context.SaveChanges();
                //Session["Email"] = a.Email.ToString();
                //Session["Username"] = a.FullName.ToString();
                ViewBag.ErrorMsg = "Create Account Success";
            }
            return View();
        }
        public IActionResult Logout()
        {
            return View();
        }

        #region will delete when merge
        public IActionResult Admin()
        {
            return View();
        }
        public IActionResult Employee()
        {
            return View();
        }
        #endregion
    }
}
