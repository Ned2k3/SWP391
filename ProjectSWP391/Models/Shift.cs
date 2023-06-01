using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Shift
    {
        public Shift()
        {
            Bookings = new HashSet<Booking>();
        }

        public int ShiftId { get; set; }
        public DateTime ShiftDay { get; set; }
        public int ShiftNumber { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
