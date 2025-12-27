using LoginDotnet.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginDotnet.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider) // only run in first time
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roles = { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (await roleManager.RoleExistsAsync(role))
                    {
                        return;
                    }
                }

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }


                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var userList = await userManager.Users.ToListAsync();
                foreach (var user in userList)
                {
                    await userManager.AddToRoleAsync(user, "User");

                }
            }

        }
    }

}
