using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;


namespace ProjectSWP391.Controllers
{
    
    public class BookingController : Controller
    {
        [BindProperty(SupportsGet = true)]
        public int serviceId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int empID { get; set; }

        [BindProperty]
        public Booking? booking { get; set; } = new Booking();

        public Account? customer { get; set; }

        public IActionResult Load(int sID)
        {
            ViewData["Title"] = "Booking service";
            //get current customer
            Account? acc = Global.CurrentUser;
            //get selected service
            Service? sv;
            using (var context = new SWP391Context())
            {
                sv = context.Services.Where(i => i.ServiceId == sID).FirstOrDefault();
            }
            booking.Customer = acc;
            booking.Service = sv;
            return View(booking);
        }

        //when guest provide their details
        [HttpPost]
        public IActionResult Load()
        {
            string fname = Request.Form["fullname"];
            string email = Request.Form["email"];
            string phone = Request.Form["phone"];
            string serviceID = Request.Form["serviceID"];
            Account acc = new Account()
            {
                FullName = fname,
                Email = email,
                Phone = Convert.ToInt32(phone)
            };
            Global.CurrentUser = acc;
            return Load(serviceId);
        }
    }
}
