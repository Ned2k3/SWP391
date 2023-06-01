using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models.Entity
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public string? Content { get; set; }
        public int? AccountId { get; set; }
        public int? ServiceId { get; set; }
        public int? ProductId { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Service? Service { get; set; }
    }
}
