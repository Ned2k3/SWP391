using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using ProjectSWP391.Models.Library;

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
            if (string.IsNullOrWhiteSpace(a.Email))
            {
                //if user doesnt Enter tai khoan
                ViewBag.ErrorMsg = "User must enter an Email and Pasword!";
                return View();
            }
            //Decryption here
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
                Global.CurrentUser = account;
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
                    return RedirectToAction("LandingPage","CustomerManagement");
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
            else if(!string.IsNullOrEmpty(email))
            {

                if (ModelState.IsValid)
                {
                    //encryption
                    string encrypted = EncryptionHelper.Encrypt(password);
                    Account c = new Account();
                    c.Password = encrypted;
                    c.Email = email;
                    c.IsActive = true;

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
        public IActionResult ForgetPassword(string email)
        {
           

            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.ErrorMsg = "Email is null or wrong format, please enter corrected email";
                return View();
            }

            var a = context.Accounts.FirstOrDefault(x => x.Email == email);
            if(a == null)
            {
                ViewBag.ErrorMsg = "Email does not exist, please enter another email or create a new Email";
                return View();
            }
            if (a.Role != null)
            {
                ViewBag.SuccessMsg = "Email forgoting password have sent to the Admin, please contact Admin for more information";
                return View();
            }
            


            //code nay tao captcha va gan no vao session
            string captcha = CaptchaGeneration.GenerateCaptcha();
            //var session = HttpContext.Session;
            //session.SetString("captcha", captcha);
            string fromMail = "smartbeautygroup5@gmail.com";
            string fromPassword = "123ABCGroup5";

            MailMessage message = new MailMessage();
            message.From = new MailAddress(fromMail);
            message.Subject = "Forgetting Password";
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body>" +
                           "<h1 style=\"color: Green\">Hi " + email + "</h1>" +
                           "<h4> You've successfully changed your password</h4>" +
                           "<h4>This is your captcha for changing password"+ "<strong style=\" font-size: 24px;\" >" + captcha+ "</strong>" + "</h4>"+
                           "<h4> If this wasn't done by you, please immediately reset the password </h4>" +
                           "<h4>To reset your password, please follow the link below:</h4>" +
                           //"<a href=\"\">Click here</a>" +
                           "<h1>Best regards,</h1>"+ "<h1 style=\"color: #2E83FF>SmartBeauty</h1>"+
                           "</body></html>";
            message.IsBodyHtml = true;
            using var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = true,
            };

            smtpClient.Send(message);
            ViewBag.SuccessMsg = "Email have sent";
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
