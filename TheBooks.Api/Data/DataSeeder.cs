using Microsoft.EntityFrameworkCore;

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

            //call seed methods
        }
        catch (Exception ex)
        {
            Console.Write(ex.Message);
        }
    }   
}