using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class ServiceCategory
    {
        public ServiceCategory()
        {
            Services = new HashSet<Service>();
        }

        public int ScategoryId { get; set; }
        public string ScategoryName { get; set; } = null!;

        public virtual ICollection<Service> Services { get; set; }
    }
}
