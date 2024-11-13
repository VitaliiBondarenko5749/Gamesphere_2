using JWTAuthentication.Data;
using JWTAuthentication.Data.Models;
using JWTAuthentication.Helpers;
using Microsoft.AspNetCore.Identity;

namespace JWTAuthentication.Seeding;

public static class SeedData
{
    public static async Task InitializeDatabase(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
    {
        if (!userManager.Users.Any() && !roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new IdentityRole { Name = UserRoles.User });
            await roleManager.CreateAsync(new IdentityRole { Name = UserRoles.Admin });
            await roleManager.CreateAsync(new IdentityRole { Name = UserRoles.Creator });

            User user = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "vitalii______",
                Email = "gamesphere7438@gmail.com",
                ImageDirectory = "/UserIcons/No-icon-image.png",
                EmailConfirmed = true
            };

            string password = "AaBb01?_";

            await userManager.CreateAsync(user, password);

            await userManager.AddToRoleAsync(user, UserRoles.Creator);

            user = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "testCreator",
                Email = "testCreator@gmail.com",
                ImageDirectory = "/UserIcons/No-icon-image.png",
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, password);

            await userManager.AddToRoleAsync(user, UserRoles.Creator);

            for(int i = 1; i <= 30; ++i)
            {
                user = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = $"testUser{i}",
                    Email = $"testUser{i}@gmail.com",
                    ImageDirectory = "/UserIcons/No-icon-image.png",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, password);

                if(i % 2 == 0)
                {
                    await userManager.AddToRoleAsync(user, UserRoles.Admin);
                }
                else
                {
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}