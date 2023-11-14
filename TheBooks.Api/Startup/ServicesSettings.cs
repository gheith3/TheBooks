using Ghak.libraries.AppBase.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TheBooks.Api.Data;
using TheBooks.Api.Model;

namespace TheBooks.Api.Startup;

public static class ServicesSettings
{
    public static IServiceCollection AppMapperConfiguration(this IServiceCollection services)
    {
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        return services;
    }

    public static IServiceCollection AppDatabaseConfiguration(this IServiceCollection services)
    {
        try
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                Console.Write(AppSettingsEntrance.GetFromAppSetting("ConnectionStrings:Default"));
                options.UseNpgsql(AppSettingsEntrance.GetFromAppSetting("ConnectionStrings:Default"));
                Console.Write("Connected");
            });

            services
                .AddIdentity<AppUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 8;
                    options.User.RequireUniqueEmail = true;
                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<AppDbContext>();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return services;
    }

    
    public static IServiceCollection AppCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }
}