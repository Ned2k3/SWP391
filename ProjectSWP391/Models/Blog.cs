using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Blog
    {
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? BlogDate { get; set; }
        public int AccountId { get; set; }

        public virtual Account? Account { get; set; }
    }
}
