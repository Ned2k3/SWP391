using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using ProjectSWP391.Models.Library;
using System.Text.RegularExpressions;
using System.Security.Policy;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectSWP391.Models.ExtendedModels;

namespace ProjectSWP391.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SWP391_V4Context context;


        public AuthenticationController(SWP391_V4Context _context)
        {
            context = _context;
        }


        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Account a)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(a.Email))
                {
                    //if user doesnt Enter tai khoan
                    ViewBag.ErrorMsg = "User must enter an Email and Pasword!";
                    return View();
                }
                //Decryption here
                a.Email = a.Email.Trim();
                a.Password = a.Password.Trim();

                string endcrypt = EncryptionHelper.Encrypt(a.Password);

                var account = context.Accounts.Where(b => b.Email == a.Email && b.Password == endcrypt).SingleOrDefault();

                //var account = context.Accounts.ToList();
                if (account == null)
                {
                    //message Login error
                    ViewBag.ErrorMsg = "Account does not exist!";
                    return View();
                }
                else
                {
                    if (account.IsActive == 0)
                    {
                        ViewBag.ErrorMsg = "Your account has been deacctive, please contact admin for more detail";
                        return View();
                    }

                    Global.CurrentUser = account;
                    var claims = new[] { new Claim(ClaimTypes.Role, account.Role.ToString()) };
                    var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomApiKeyAuth"));
                    DateTimeOffset timerStop = DateTimeOffset.UtcNow.AddMinutes(30);
                    await HttpContext.SignInAsync("Auth", principal, new AuthenticationProperties { AllowRefresh = true, ExpiresUtc = timerStop, IsPersistent = true });
                    //message
                    if (account.Role == 1)
                    {
                        return RedirectToAction("Admin");
                    }
                    else if (account.Role == 2)
                    {
                        return RedirectToAction("Employee");
                    }
                    else
                    {
                        if (account.FullName == null)
                        {
                            return RedirectToAction("CompleteCustomerProfile", "Profile", new { id = account.AccountId });
                        }
                        string[] msg = { "Welcome Back", $"{Global.CurrentUser.FullName}" };
                        return RedirectToAction("LandingPage", "CustomerManagement", new { message = msg });
                    }
                    //return View();
                }
            }
            catch (Exception ex)
            {
                return Redirect("~/Error/CommonError");
            }

        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(string email, string password, string rePassword)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            // Create a Regex object with the email pattern
            Regex regex = new Regex(emailPattern);
            email = email.Trim();
            if (rePassword != password)
            {
                ViewBag.ErrorMsg = "Re enter password does not match password. Please enter again ";
                return View();
            }
            if (context.Accounts.Any(x => x.Email == email))
            {
                ViewBag.ErrorMsg = "Account has existed";
                return View();
            }
            else if (!string.IsNullOrEmpty(email))
            {

                if (regex.IsMatch(email))
                {
                    //encryption
                    string encrypted = EncryptionHelper.Encrypt(password);
                    Account c = new Account();
                    c.Password = encrypted;
                    c.Email = email;
                    c.IsActive = 1;

                    context.Add(c);
                    context.SaveChanges();
                    //Session["Email"] = a.Email.ToString();
                    //Session["Username"] = a.FullName.ToString();
                    ViewBag.SuccessMsg = "Create Account Success";
                }
                else
                {
                    ViewBag.ErrorMsg = "User enter wrong email format";
                }
            }
            else
            {
                ViewBag.ErrorMsg = "User enter wrong email format";
            }
            return View();
        }
        public IActionResult Logout()
        {
            //xoa sesion, logout
            //quay ve trang list de login lai
            //HttpContext.Session.Get().Clear();
            Global.CurrentUser = null;
            return RedirectToAction("LandingPage", "CustomerManagement");
        }

        public IActionResult ForgetPassword()
        {

            return View();
        }

        [HttpPost]
        public IActionResult ForgetPassword(string email, string checkC)
        {
            ViewBag.Email = email;
            email = email.Trim();
            //check nguoi dung da nhap cai gi
            if (!string.IsNullOrWhiteSpace(checkC))
            {

                string captchaSession = HttpContext.Session.GetString("captcha").ToString();
                string emailer = HttpContext.Session.GetString("accountSession").ToString();
                string captchaExpiration = HttpContext.Session.GetString("captchaExpiration");

                if (string.IsNullOrWhiteSpace(captchaSession))
                {
                    ViewBag.ErrorMsg = "There is no captcha that have been sent to Email, please re-sent";
                    return View();
                }
                else
                {
                    if (captchaSession == checkC && email == emailer)
                    {
                        if (captchaExpiration == null || DateTime.UtcNow > DateTime.Parse(captchaExpiration))
                        {
                            ViewBag.ErrorMsg = "Captcha expired, please try send email again";
                            ViewBag.CheckCaptcha = "false";
                            HttpContext.Session.Remove("captcha");
                            HttpContext.Session.Remove("accountSession");
                            return View();
                        }
                        else
                        {
                            ViewBag.SuccessMsg = "New Password have matched. Reset password successfully into new Password";
                            var PasswordChange = context.Accounts.FirstOrDefault(x => x.Email == email);
                            //change password
                            PasswordChange.Password = EncryptionHelper.Encrypt(captchaSession);
                            context.SaveChanges();
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Captcha and/or Account does not matched";
                        return View();
                    }
                }

            }

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ErrorMsg = "Email is null or wrong format, please enter corrected email";
                return View();
            }

            var a = context.Accounts.FirstOrDefault(x => x.Email == email);
            if (a == null)
            {
                ViewBag.ErrorMsg = "Email does not exist, please enter another email or create a new Email";
                return View();
            }
            if (a.IsActive == 0)
            {
                ViewBag.ErrorMsg = "Inactive User cannot reset password. Please contact admin for more detail";
                return View();
            }
            if (a.Role != null)
            {
                ViewBag.SuccessMsg = "Email forgoting password have been sent to the Admin, please contact an Admin for more information";
                a.IsActive = 2;
                context.SaveChanges();
                return View();
            }

            //code nay tao captcha va gan no vao session
            //session nay duoc gan x minute
            string captcha = CaptchaGeneration.GenerateCaptcha();
            HttpContext.Session.SetString("captcha", captcha);
            HttpContext.Session.SetString("accountSession", email);
            HttpContext.Session.SetString("captchaExpiration", DateTime.UtcNow.AddMinutes(5).ToString());
            ViewBag.CheckCaptcha = "true";
            ViewBag.Captcha = captcha;
            string fromMail = "smartbeautygroup5@gmail.com";
            //quantrong la buoc mat khau nay
            string fromPassword = "tunudlgpqbgqwbpz"; //123ABCGroup5

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Forgetting Password";
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body style=\"font-family: Arial, sans-serif;\">" +
                            "<h1 style=\"color: #007BFF; font-size: 28px;\">Hi " + email + "</h1>" +
                            "<p style=\"font-size: 18px; color: #333333;\">Congratulations! Your email password has been successfully changed.</p>" +
                            "<p style=\"font-size: 20px; color: #007BFF;\">This is your new Password for the new password field:</p>" +
                            "<strong style=\"font-size: 36px; color: #FF4500;\">" + captcha + "</strong>" +
                            "<p style=\"font-size: 18px; color: #333333;\">If you did not initiate this password change, please reset your password immediately.</p>" +
                            "<h2 style=\"font-size: 22px; color: #333333;\">Best regards,</h2>" +
                            "<h2 style=\"color: #007BFF;\">SmartBeauty</h2>" +
                            "</body></html>";
            message.IsBodyHtml = true;
            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
                UseDefaultCredentials = false,


            };

            smtpClient.Send(message);
            ViewBag.SuccessMsg = "Email have sent";

            return View();


        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string email, string password, string? rePassword, string? newRePassword)
        {
            email = email.Trim();
            password = password.Trim();

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(rePassword) || string.IsNullOrEmpty(newRePassword))
            {
                ViewBag.ErrorMsg = "Fields cannot be null or a space input please try a different password";
                return View();
            }

            var a = context.Accounts.FirstOrDefault(x => x.Email == email && x.Password == EncryptionHelper.Encrypt(password));
            if (a == null)
            {
                ViewBag.ErrorMsg = "Account/Password is incorrected, please try again.";
                return View();
            }
            else
            {
                if (password == rePassword)
                {
                    ViewBag.ErrorMsg = "New password must be different from current password ";
                    return View();
                }
                if (rePassword != newRePassword)
                {
                    ViewBag.ErrorMsg = "New Password must matched the Re-input new password";
                    return View();
                }
                else
                {
                    a.Password = EncryptionHelper.Encrypt(rePassword);
                    context.SaveChanges();
                    ViewBag.SuccessMsg = "Password changed successfully";
                    return View();
                }
            }
            return View();
        }

        #region will delete when merge
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Admin()
        {
            return RedirectToAction("DashBoard", "AdminManagement");
        }

        [Authorize(AuthenticationSchemes = "Auth", Roles = "2")]
        public IActionResult Employee()
        {
            return RedirectToAction("DashBoard", "EmployeeManagement");
        }

        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult AdminPassword()
        {
            var a = context.Accounts.Where(acc => acc.IsActive == 2).ToList();

            return View(a);
        }

        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult ApprovePassword(string email)
        {
            var a = context.Accounts.Where(e => e.Email == email).SingleOrDefault();
            a.Password = EncryptionHelper.Encrypt("123");
            a.IsActive = 1;
            ViewBag.SuccessMsg = "Password change successfully for" + email;
            context.SaveChanges();
            return RedirectToAction("AdminPassword");
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult DeclinePassword(string email)
        {
            var a = context.Accounts.Where(e => e.Email == email).SingleOrDefault();
            a.IsActive = 1;
            ViewBag.SuccessMsg = "Password change decline" + email;
            context.SaveChanges();
            return RedirectToAction("AdminPassword");
        }

        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult ProductRecord(int? year)
        {
            List<RevenueData> productData = new List<RevenueData>();
            List<RevenueData> serviceData = new List<RevenueData>();

            if (year != null)
            {
                ViewBag.Year = year;
                var a = GetProductRevenueByMonth(year);
                var b = GetServiceRevenueByMonth(year);
                productData = FillRevenueDataForAllMonths(a);
                serviceData = FillRevenueDataForAllMonths(b);
            }
            else
            {
                ViewBag.Year = Convert.ToInt32(DateTime.Now.Year);
                var a = GetProductRevenueByMonth(null);
                var b = GetServiceRevenueByMonth(null);
                productData = FillRevenueDataForAllMonths(a);
                serviceData = FillRevenueDataForAllMonths(b);
            }

            var userPurchases = context.Accounts
                    // Replace with the user email you want to retrieve purchases for
                    .Join(context.Orders, account => account.AccountId, order => order.AccountId, (account, order) => new { account, order })
                    .Join(context.OrderDetails, combined => combined.order.OrderId, orderDetail => orderDetail.OrderId, (combined, orderDetail) => new { combined.account, combined.order, orderDetail })
                    .Join(context.Products, combined => combined.orderDetail.ProductId, product => product.ProductId, (combined, product) => new { combined.account, combined.order, combined.orderDetail, product })
                    .OrderByDescending(combined => combined.order.OrderDate)
                    .Select(result => new
                    {

                        BuyerEmail = result.account.Email,
                        OrderId = result.order.OrderId,
                        ProductName = result.product.ProductName,
                        OrderDate = result.order.OrderDate,
                        Amount = result.orderDetail.Amount,
                        Content = result.order.Content,
                        Total = result.orderDetail.Amount * result.product.Price
                    })
                    .ToList();
            List<ProductUserData> listUser = new List<ProductUserData>();
            foreach (var newUser in userPurchases)
            {
                listUser.Add(new ProductUserData(newUser.BuyerEmail, newUser.ProductName, newUser.OrderId, newUser.OrderDate.Value, newUser.Amount, newUser.Content, newUser.Total));
            }

            var viewModel = new MyViewModel
            {
                ProductRevenueByMonth = productData,
                ServiceRevenueByMonth = serviceData,
                ProductUserDatas = listUser
            };


            return View(viewModel);

        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult ServiceRecord(int? year)
        {
            List<RevenueData> productData = new List<RevenueData>();
            List<RevenueData> serviceData = new List<RevenueData>();

            if (year != null)
            {
                ViewBag.Year = year;
                var a = GetProductRevenueByMonth(year);
                var b = GetServiceRevenueByMonth(year);
                productData = FillRevenueDataForAllMonths(a);
                serviceData = FillRevenueDataForAllMonths(b);
            }
            else
            {
                ViewBag.Year = Convert.ToInt32(DateTime.Now.Year);
                var a = GetProductRevenueByMonth(null);
                var b = GetServiceRevenueByMonth(null);
                productData = FillRevenueDataForAllMonths(a);
                serviceData = FillRevenueDataForAllMonths(b);
            }

            var serviceList = context.ServiceLists.Include(c => c.Booking).OrderBy(c => c.Booking.BookingDate).Include(c => c.Service).ToList();


            var service = context.Services.ToList();
            List<ServiceUserData> listUser = new List<ServiceUserData>();
            foreach (var newUser in serviceList)
            {
                listUser.Add(new ServiceUserData(getEmailUser(newUser.Booking.CustomerId), getEmailUser(newUser.Booking.EmployeeId),
                    newUser.BookingId, newUser.Booking.BookingDate, newUser.Booking.Shift,
                    newUser.Service.ServiceName, newUser.Service.Price, newUser.Booking.Content));
            }

            var viewModel = new MyViewModel
            {
                ProductRevenueByMonth = productData,
                ServiceRevenueByMonth = serviceData,
                ServiceUserDatas = listUser
            };

            return View(viewModel);
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult DetailShopList(int order)

        {
            var getOrder = context.Orders.SingleOrDefault(or => or.OrderId == order);
            Account getAccount = context.Accounts.Where(ac => ac.AccountId == getOrder.AccountId).SingleOrDefault();
            //var a = getOrder;

            ViewBag.Email = "" + getAccount.Email;
            ViewBag.Username = "" + getAccount.FullName;
            ViewBag.Content = "" + getOrder.Content;
            var orderDetails = context.OrderDetails
                    .Include(od => od.Order)
                    .Include(od => od.Product)
                    .Where(od => od.OrderId == order)
                    .ToList();

            return View(orderDetails);
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult DetailServiceList(int booking)
        {
            var serviceList = context.ServiceLists.Include(c => c.Booking).OrderBy(c => c.Booking.BookingDate)
                .Include(c => c.Service)
                .Where(c => c.BookingId == booking)
                .ToList();
            var a = serviceList.First();
            Account customer = getUser(a.Booking.CustomerId);
            Account employee = getUser(a.Booking.EmployeeId);

            ViewBag.Customer = customer.FullName;
            ViewBag.CustomerEmail = customer.Email;
            ViewBag.Employee = employee.FullName;
            ViewBag.EmployeeEmail = employee.FullName;
            return View(serviceList);
        }

        #endregion

        #region Some method to call
        public string getEmailUser(int id)
        {
            Account a = context.Accounts.Where(a => a.AccountId == id).SingleOrDefault();
            return a.Email;
        }
        public Account getUser(int id)
        {
            return context.Accounts.Where(a => a.AccountId == id).SingleOrDefault();
        }
        public List<RevenueData> GetProductRevenueByMonth(int? year)
        {
            if (year != null)
            {
                List<RevenueData> revenueDataList = new List<RevenueData>();
                var revenueProductByMonth = context.Accounts
                       .Join(context.Orders, account => account.AccountId, order => order.AccountId, (account, order) => new { account, order })
                       .Join(context.OrderDetails, combined => combined.order.OrderId, orderDetail => orderDetail.OrderId, (combined, orderDetail) => new { combined.account, combined.order, orderDetail })
                       .Join(context.Products, combined => combined.orderDetail.ProductId, product => product.ProductId, (combined, product) => new { combined.account, combined.order, combined.orderDetail, product })
                       .Where(combined => combined.order.OrderDate.Value.Year == year) // Filter by desired year
                       .GroupBy(combined => combined.order.OrderDate.Value.Month)
                       .Select(group => new
                       {
                           Month = group.Key,
                           Revenue = group.Sum(combined => combined.orderDetail.Amount * combined.product.Price)
                       })
                       .OrderBy(group => group.Month)
                       .ToList();
                foreach (var revenue in revenueProductByMonth)
                {
                    var a = new RevenueData(revenue.Month, revenue.Revenue);
                    revenueDataList.Add(a);
                }
                return revenueDataList;
            }
            else
            {
                List<RevenueData> revenueDataList = new List<RevenueData>();
                var revenueProductByMonth = context.Accounts
                       .Join(context.Orders, account => account.AccountId, order => order.AccountId, (account, order) => new { account, order })
                       .Join(context.OrderDetails, combined => combined.order.OrderId, orderDetail => orderDetail.OrderId, (combined, orderDetail) => new { combined.account, combined.order, orderDetail })
                       .Join(context.Products, combined => combined.orderDetail.ProductId, product => product.ProductId, (combined, product) => new { combined.account, combined.order, combined.orderDetail, product })
                       .Where(combined => combined.order.OrderDate.Value.Year == DateTime.Now.Year) //can changed based on year
                       .GroupBy(combined => combined.order.OrderDate.Value.Month)
                       .Select(group => new
                       {
                           Month = group.Key,
                           Revenue = group.Sum(combined => combined.orderDetail.Amount * combined.product.Price)
                       })
                       .OrderBy(group => group.Month)
                       .ToList();
                foreach (var revenue in revenueProductByMonth)
                {
                    var a = new RevenueData(revenue.Month, revenue.Revenue);
                    revenueDataList.Add(a);
                }
                return revenueDataList;
            }
        }

        public List<RevenueData> GetServiceRevenueByMonth(int? year)
        {
            if (year == null)
            {
                List<RevenueData> revenueDataList = new List<RevenueData>();
                var revenueServiceByMonth = context.Accounts
                    .Join(context.Bookings, account => account.AccountId, booking => booking.EmployeeId, (account, booking) => new { account, booking })
                    .Join(context.ServiceLists, combined => combined.booking.BookingId, serviceList => serviceList.BookingId, (combined, serviceList) => new { combined.account, combined.booking, serviceList })
                    .Join(context.Services, combined => combined.serviceList.ServiceId, service => service.ServiceId, (combined, service) => new { combined.account, combined.booking, combined.serviceList, service })
                    .Where(combined => combined.booking.BookingDate.Year == DateTime.Now.Year) //change this if needed?
                    .GroupBy(combined => combined.booking.BookingDate.Month)
                    .Select(group => new
                    {
                        Month = group.Key,
                        Revenue = group.Sum(combined => combined.service.Price)
                    })
                    .OrderBy(group => group.Month)
                    .ToList();
                foreach (var revenue in revenueServiceByMonth)
                {
                    var a = new RevenueData(revenue.Month, revenue.Revenue);
                    revenueDataList.Add(a);
                }
                return revenueDataList;
            }
            else
            {

                List<RevenueData> revenueDataList = new List<RevenueData>();
                var revenueServiceByMonth = context.Accounts
                    .Join(context.Bookings, account => account.AccountId, booking => booking.EmployeeId, (account, booking) => new { account, booking })
                    .Join(context.ServiceLists, combined => combined.booking.BookingId, serviceList => serviceList.BookingId, (combined, serviceList) => new { combined.account, combined.booking, serviceList })
                    .Join(context.Services, combined => combined.serviceList.ServiceId, service => service.ServiceId, (combined, service) => new { combined.account, combined.booking, combined.serviceList, service })
                    .Where(combined => combined.booking.BookingDate.Year == year) // Filter by desired year
                    .GroupBy(combined => combined.booking.BookingDate.Month) // Group by month
                    .Select(group => new
                    {
                        Month = group.Key,
                        Revenue = group.Sum(combined => combined.service.Price)
                    })
                    .OrderBy(group => group.Month)
                    .ToList();
                foreach (var revenue in revenueServiceByMonth)
                {
                    var a = new RevenueData(revenue.Month, revenue.Revenue);
                    revenueDataList.Add(a);
                }
                return revenueDataList;
            }
        }
        private List<RevenueData> FillRevenueDataForAllMonths(List<RevenueData> revenueData)
        {
            var filledData = new List<RevenueData>();

            for (int month = 1; month <= 12; month++)
            {
                var matchingData = revenueData.FirstOrDefault(d => d.Month == month);

                if (matchingData != null)
                {
                    filledData.Add(matchingData);
                }
                else
                {
                    filledData.Add(new RevenueData(month, 0));
                }
            }

            return filledData;
        }
        #endregion
    }
}
