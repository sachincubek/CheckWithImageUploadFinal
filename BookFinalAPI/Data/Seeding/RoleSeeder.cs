using BookFinalAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace BookFinalAPI.Data.Seeding
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            string[] roles = new[] { "Admin", "Buyer", "Donor", "Renter" };
            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));
            }

            // Optionally create default admin from config
            // var adminEmail = "admin@domain.com"; // read from config
            // if (await userManager.FindByEmailAsync(adminEmail) == null) { ... create and assign Admin role ... }
        }
    }
}
