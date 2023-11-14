using Ghak.libraries.AppBase.Extensions;
using Ghak.libraries.AppBase.Utils;
using TheBooks.Api.Startup;

try
{
    var builder = WebApplication.CreateBuilder(args);
    AppSettingsEntrance.SetAppSetting(builder.Configuration);


    // Add services to the container.
    builder.Services
        .AddHttpContextAccessor()
        .RepositoriesRegistration()
        .AppCors()
        .AddHttpClient()
        .AppDatabaseConfiguration()
        .AddJwtAuthentication()
        .AppMapperConfiguration()
        .AppSwaggerDocSetting(true)
        .AddControllers();


    var app = builder.Build();
    //to solve postgres db issue with datetime
    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    app.AppDatabaseSeed();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseAppSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Unhandled exception: {ex.Message}");
}
finally
{
    Console.WriteLine("Shut down complete");
}