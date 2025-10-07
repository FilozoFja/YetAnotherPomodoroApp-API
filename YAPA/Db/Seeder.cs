using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using YAPA.Models;

namespace YAPA.Db;

public class Seeder
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<UserModel>>();
        
        if (!context.Users.Any())
        {
            var user = new UserModel
            {
                Email = "admin@admin.com",
                UserName = "admin",
                EmailConfirmed = true 
            };
            
            var result = await userManager.CreateAsync(user, "Admin123!");
            
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
            }
        }
    }
}