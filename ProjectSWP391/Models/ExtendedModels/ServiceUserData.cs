using ProjectSWP391.Models;

namespace ProjectSWP391.Models.ExtendedModels
{
    public class ServiceUserData
    {
        public string CustomerEmail { get; set; }
        public string EmployeeEmail { get; set; }
        public int BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public int Shift { get; set; }  
        public string ServiceName { get; set; }
        
        public decimal Price { get; set; }
        public string Note { get; set; }

        public ServiceUserData(string customerEmail, string employeeEmail, int bookingId, DateTime bookingDate, int shift, string serviceName, decimal price, string note)
        {
            CustomerEmail = customerEmail;
            EmployeeEmail = employeeEmail;
            BookingId = bookingId;
            BookingDate = bookingDate;
            Shift = shift;
            ServiceName = serviceName;
            Price = price;
            Note = note;
        }

      
    }
}
