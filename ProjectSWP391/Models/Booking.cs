using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Booking
    {
        public int BookingId { get; set; }
        public int? EmployeeId { get; set; }
        public int? CustomerId { get; set; }
        public int? ServiceId { get; set; }
        public string Content { get; set; }
        public DateTime? BookingDate { get; set; }
        public int? Shift { get; set; }

        public virtual Account Customer { get; set; }
        public virtual Account Employee { get; set; }
        public virtual Service Service { get; set; }
    }
}
