using TheBooks.Api.Repositories.Auth;

namespace TheBooks.Api.Startup;

public static class AppRegistrations
{
    public static IServiceCollection RepositoriesRegistration(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        return services;
    }
    
    public static IServiceCollection ServicesRegistration(this IServiceCollection services)
    {

        return services;
    }
}