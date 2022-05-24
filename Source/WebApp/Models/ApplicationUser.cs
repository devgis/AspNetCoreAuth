using Microsoft.AspNetCore.Identity;

namespace WebApp.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string CustomTag { get; set; }
    }
}
