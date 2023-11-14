using TheBooks.Api.Data;

namespace TheBooks.Api.Startup;

public static class AppSettings
{
    public static WebApplication AppDatabaseSeed(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        DataSeeder.Initialize(services).Wait();
        return app;
    }
}