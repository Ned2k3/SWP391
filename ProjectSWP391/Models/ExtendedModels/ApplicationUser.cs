using System.Data.Common;
using Microsoft.AspNetCore.Identity;

namespace ProjectSWP391.Models.ExtendedModels
{
    public class ApplicationUser : IdentityUser
    {
        public string email { get; set; } = null;

    }
}
