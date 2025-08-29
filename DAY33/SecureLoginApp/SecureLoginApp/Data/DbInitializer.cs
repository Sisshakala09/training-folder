using Microsoft.AspNetCore.Identity;

namespace SecureLoginApp.Data
{
    public class DbInitializer
    {
        public static async Task SeedRolesAndUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Roles
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            if (!await roleManager.RoleExistsAsync("User"))
                await roleManager.CreateAsync(new IdentityRole("User"));

            // Admin
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new IdentityUser { UserName = "admin", Email = "admin@test.com" };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // User
            var user = await userManager.FindByNameAsync("user1");
            if (user == null)
            {
                user = new IdentityUser { UserName = "user1", Email = "user1@test.com" };
                await userManager.CreateAsync(user, "User@123");
                await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
