using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;
using System.Runtime.Versioning;

namespace ProjectSWP391.Controllers
{

    public class BookingController : Controller
    {
        [BindProperty]
        public Booking? booking { get; set; } = new Booking();

        public IActionResult Load(int sID)
        {
            var session = HttpContext.Session;
            //get current customer
            Account? acc = Global.CurrentUser;
            if (acc != null)
            {
                booking.Customer = acc;
            }
            if(sID != 0)
            {
                using (var context = new SWP391Context())
                {
                    Service? sv = context.Services.Where(i => i.ServiceId == sID).FirstOrDefault();
                    if (sv != null)
                    {
                        booking.Service = sv;
                        session.SetString("serviceID", sID.ToString());
                    }
                }
            }
            return View(booking);
        }

        [HttpPost]
        public IActionResult LoadByPost()
        {
            var session = HttpContext.Session;
            Account? acc = new Account();
            int step = Convert.ToInt32(Request.Form["step"]);
            //guest enter phone number from landing page
            if (step == 0)
            {
                int phoneNumber = Convert.ToInt32(Request.Form["phone"]);
                if (session.GetString("phone") != null)
                {
                    session.Clear();
                }
                using (var context = new SWP391Context())
                {
                    var account = context.Accounts.Where(a => a.AccountId == phoneNumber).FirstOrDefault();
                    session.SetString("phone", phoneNumber.ToString());
                    if (account != null)
                    {
                        booking.Customer = account;
                    }
                    else
                    {
                        acc.Phone = phoneNumber;
                        booking.Customer = acc;
                    }
                }
            }

            //guest book from service list and have to fill information
            else if (step == 1)
            {
                int phoneNumber = Convert.ToInt32(Request.Form["phone"]);
                int sID = Convert.ToInt32(session.GetString("serviceID"));
                string? name = Request.Form["fname"].ToString().Trim();
                string? email = Request.Form["email"].ToString().Trim();
                session.SetString("phone", phoneNumber.ToString());
                using (var context = new SWP391Context())
                {
                    var account = context.Accounts.Where(a => a.Phone == phoneNumber).FirstOrDefault();
                    Service? sv = context.Services.Where(i => i.ServiceId == sID).FirstOrDefault();
                    if (account != null)
                    {
                        booking.Customer = account;
                    }
                    else
                    {
                        acc.Phone = phoneNumber;
                        if (name != null)
                        {
                            session.SetString("fname", name);
                            acc.FullName = name;
                        }
                        if (email != null)
                        {
                            session.SetString("email", email);
                            acc.Email = email;
                        }
                        booking.Customer = acc;
                    }
                    if (sv != null)
                    {
                        booking.Service = sv;
                    }
                }

            }
            return View("/Views/Booking/Load.cshtml", booking);
        }
    }
}
