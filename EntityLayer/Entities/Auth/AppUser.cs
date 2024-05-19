using Microsoft.AspNetCore.Identity;

namespace EntityLayer.Entities.Auth
{
    public class AppUser : IdentityUser
    {
        public string? ProfileImagePath { get; set; }
    }
}
