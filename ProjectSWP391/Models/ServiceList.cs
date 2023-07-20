using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class ServiceList
    {
        public int ServiceListId { get; set; }
        public int BookingId { get; set; }
        public int ServiceId { get; set; }

        public virtual Booking Booking { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
