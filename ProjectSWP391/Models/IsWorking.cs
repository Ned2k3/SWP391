using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class IsWorking
    {
        public int WorkingId { get; set; }
        public int AccountId { get; set; }
        public int Status { get; set; }
        public DateTime? WorkingDay { get; set; }

        public virtual Account Account { get; set; } = null!;
    }
}
