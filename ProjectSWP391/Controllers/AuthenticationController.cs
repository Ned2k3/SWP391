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
        public IActionResult Register(string email, string password, string rePassword)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            // Create a Regex object with the email pattern
            Regex regex = new Regex(emailPattern);

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
            else if(!string.IsNullOrEmpty(email))
            {

                if (regex.IsMatch(email))
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
        public IActionResult ForgetPassword(string email, string checkC)
        {
          
            //check nguoi dung da nhap cai gi
            if (!string.IsNullOrWhiteSpace(checkC))
            {
                string captchaSession = HttpContext.Session.GetString("captcha").ToString();
                if (string.IsNullOrWhiteSpace(captchaSession)) 
                {
                    ViewBag.ErrorMsg = "There is no captcha that have been sent to Email, please re-sent";
                    return View(); 
                }
                else if (captchaSession==checkC)
                {
                    ViewBag.SuccessMsg = "Captcha have matched. Reset password successful";
                    return View();
                }
                ViewBag.ErrorMsg = "Have captcha";
                
                return View();
            }

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
                ViewBag.SuccessMsg = "Email forgoting password have been sent to the Admin, please contact an Admin for more information";
                return View();
            }
            


            //code nay tao captcha va gan no vao session
            string captcha = CaptchaGeneration.GenerateCaptcha();
            HttpContext.Session.SetString("captcha", captcha);
            HttpContext.Session.SetString("accountSession", email);
            ViewBag.CheckCaptcha = "true";

        //    string fromMail = "smartbeautygroup5@gmail.com";
        //    //quantrong la buoc mat khau nay
        //    string fromPassword = "tunudlgpqbgqwbpz"; //123ABCGroup5

        //    MailMessage message = new MailMessage();
        //    message.From = new MailAddress(fromMail);
        //    message.Subject = "Forgetting Password";
        //    message.To.Add(new MailAddress(email));
        //    message.Body =  "<html><body style=\"font-family: Arial, sans-serif;\">" +
        //                    "<h1 style=\"color: #007BFF; font-size: 28px;\">Hi " + email + "</h1>" +
        //                    "<p style=\"font-size: 18px; color: #333333;\">Congratulations! Your email password has been successfully changed.</p>" +
        //                    "<p style=\"font-size: 20px; color: #007BFF;\">This is your captcha for the password change:</p>" +
        //                    "<strong style=\"font-size: 36px; color: #FF4500;\">" + captcha + "</strong>" +
        //                    "<p style=\"font-size: 18px; color: #333333;\">If you did not initiate this password change, please reset your password immediately.</p>" +
        //                    "<h2 style=\"font-size: 22px; color: #333333;\">Best regards,</h2>" +
        //                    "<h2 style=\"color: #007BFF;\">SmartBeauty</h2>" +
        //                    "</body></html>";
        //    message.IsBodyHtml = true;
        //    using var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new NetworkCredential(fromMail, fromPassword),
        //        EnableSsl = true,
        //        UseDefaultCredentials = false,
               

        //};

        //    smtpClient.Send(message);
            ViewBag.SuccessMsg = "Email have sent";
            
            return View();
            //dang ki captcha + session theo tai khoan vao day vao day
            
        }
      
        
        public IActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(string email, string password, string rePassword, string newRePassword)
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
