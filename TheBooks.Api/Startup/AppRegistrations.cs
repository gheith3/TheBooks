using TheBooks.Api.Repositories.Auth;
using TheBooks.Api.Repositories.BookCollections;
using TheBooks.Api.Services.Auth;

namespace TheBooks.Api.Startup;

public static class AppRegistrations
{
    public static IServiceCollection RepositoriesRegistration(this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IBookCollectionsRepository, BookCollectionsRepository>();
        return services;
    }
    
    public static IServiceCollection ServicesRegistration(this IServiceCollection services)
    {
        services.AddScoped<IAuthUserServices, AuthUserServices>();
        return services;
    }
}