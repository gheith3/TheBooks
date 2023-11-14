using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBooks.Api.Helpers;
using TheBooks.Api.Model;

namespace TheBooks.Api.Data;


public static class DataSeeder
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        try
        {
            //init database, seed master data to database
            var context = serviceProvider.GetRequiredService<AppDbContext>();

            //migrate database
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            //call seed method
            await SeedUsersRoles(context, serviceProvider);
            await SeedUsers(context, serviceProvider);
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }   
    
        private static async Task SeedUsers(AppDbContext context, IServiceProvider serviceProvider)
    {
        try
        {
            if (context.Users.Any())
                return;

            Console.WriteLine("start user seed");

            var user = new AppUser
            {
                UserName = "root",
                Email = "root@admin.com",
                EmailConfirmed = true,
                PhoneNumber = "9651254",
                PhoneNumberConfirmed = true,
            };

            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var result = await userManager.CreateAsync(user, "P@ssw0rd");
            if (result.Succeeded)
            {
                // Assign the "User" role to the user
                await userManager.AddToRoleAsync(user, AppUsersRoles.Root);
                Console.WriteLine("user seed successfully");
                return;
            }
            
            if (result.Errors.Any())
            {
                foreach (var e in result.Errors)
                {
                    Console.WriteLine($"{e.Code}    {e.Description}");
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in seeding users, {e.Message}");
            throw;
        }
    }

    private static async Task SeedUsersRoles(AppDbContext context, IServiceProvider serviceProvider)
    {
        Console.WriteLine("start users roles seed");
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!roleManager.RoleExistsAsync(AppUsersRoles.Root).Result)
        {
            await roleManager.CreateAsync(new IdentityRole(AppUsersRoles.Root));
        }

        if (!roleManager.RoleExistsAsync(AppUsersRoles.Admin).Result)
        {
            await roleManager.CreateAsync(new IdentityRole(AppUsersRoles.Admin));
        }

        if (!roleManager.RoleExistsAsync(AppUsersRoles.User).Result)
        {
            await roleManager.CreateAsync(new IdentityRole(AppUsersRoles.User));
        }
    }
}