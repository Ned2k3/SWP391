using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Booking
    {
        public Booking()
        {
            ServiceLists = new HashSet<ServiceList>();
        }

        public int BookingId { get; set; }
        public int EmployeeId { get; set; }
        public int CustomerId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime BookingDate { get; set; }
        public int Shift { get; set; }

        public virtual Account Customer { get; set; } = null!;
        public virtual Account Employee { get; set; } = null!;
        public virtual ICollection<ServiceList> ServiceLists { get; set; }
    }
}
