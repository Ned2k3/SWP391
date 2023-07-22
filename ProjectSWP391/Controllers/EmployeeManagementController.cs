using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;

namespace ProjectSWP391.Controllers
{
    public class EmployeeManagementController : Controller
    {
        private readonly SWP391_V4Context context;

        public EmployeeManagementController()
        {
            context = new SWP391_V4Context();
        }

        public IActionResult DashBoard()
        {
            return View();
        }

        public IActionResult ViewSchedule(string? date)
        {
            List<DateTime> schedule = Global.GetSchedule();
            ViewBag.schedule = schedule;
            Account? employee = Global.CurrentUser;
            DateTime selectDate;
            if (date == null)
            {
                selectDate = DateTime.Today;
            }
            else selectDate = DateTime.Parse(date);
            ViewBag.selectDate = selectDate;
            int currentHour = DateTime.Now.Hour;
            var bookings = context.Bookings.Where(b => b.EmployeeId == employee.AccountId
                && (b.BookingDate.Date.Equals(selectDate) && b.Shift >= currentHour)).Select(b => new
                {
                    ID = b.BookingId,
                    Customer = b.Customer,
                    Employee = b.Employee,
                    Date = b.BookingDate,
                    Shift = b.Shift,
                    Selection = (from a in b.ServiceLists join c in context.Services on a.ServiceId equals c.ServiceId
                                 select c).ToList(),
                    Content = b.Content,
                });
            int count = bookings.Count();
            ViewBag.count = count;
            return View(bookings);
        }
    }
}
