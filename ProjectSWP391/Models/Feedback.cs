using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Feedback
    {
        public int FeedbackId { get; set; }
        public string Content { get; set; }
        public int? AccountId { get; set; }
        public int? ServiceId { get; set; }
        public int? ProductId { get; set; }
        public DateTime? Date { get; set; }

        public virtual Account? Account { get; set; }
        public virtual Product FeedbackNavigation { get; set; } = null!;
        public virtual Service? Service { get; set; }
    }
}
