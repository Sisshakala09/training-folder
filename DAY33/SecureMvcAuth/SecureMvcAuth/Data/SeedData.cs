using Microsoft.AspNetCore.Identity;
using SecureMvcAuth.Models;

namespace SecureMvcAuth.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<Test>>();

            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin user
            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                admin = new Test { UserName = "admin", Email = "admin@example.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Normal user
            var user = await userManager.FindByNameAsync("user1");
            if (user == null)
            {
                user = new Test { UserName = "user1", Email = "user1@example.com", EmailConfirmed = true };
                var result = await userManager.CreateAsync(user, "User@123");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, "User");
            }
        }
    }
}
