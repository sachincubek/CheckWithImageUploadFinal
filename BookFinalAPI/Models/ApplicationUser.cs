using Microsoft.AspNetCore.Identity;

namespace BookFinalAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Pincode { get; set; }
        public string? ProfileImageUrl { get; set; }

        public bool IsPaid { get; set; }
    }
}
