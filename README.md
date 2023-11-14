
**Step 1: Create a new ASP.NET Core API Project**

1.1. Open a terminal and run the following command to create a new ASP.NET Core API project named TheBooks.Api within a solution named TheBooks:

    dotnet new webapi -n TheBooks.Api -o TheBooks

1.2. Change into the project directory:

    cd TheBooks

Step 2: Configure Project Settings

2.1. Configure the project for HTTPS, enable Docker support, set Docker OS to Linux, and include controllers:

    dotnet new gitignore
    dotnet publish -c Release -o out

Step 3: Install Required Packages
3.1. Install the necessary packages for the project:

    dotnet add package Ghak.libraries.AppBase
    dotnet add package AutoMapper
    dotnet add package Humanizer.Core
    dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.Design
    dotnet add package Microsoft.EntityFrameworkCore.Tools
    dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
    dotnet add package Swashbuckle.AspNetCore

3.2. Ensure that the project builds successfully:

    dotnet build

Step 4: Project Structure and Cleanup

4.1. Delete the following files and folders:

- WeatherForecastController
- TheBooks.Api.http
- WeatherForecast

4.2. Create the following folders:

- Data
- Dto
- Model
- Handlers
- Helpers
- Repositories
- Services
- Startup
- Mappers

Step 5: Docker PostgreSQL Database
5.1. Create a Docker container for PostgreSQL database:

    docker run -d -i --name TheBooksApp -p 5438:5432 -e POSTGRES_PASSWORD=postgres -e POSTGRES_USER=postgres postgres

Step 6: Update AppSettings
6.1. Update the appsettings.json and appsettings.Development.json files with the following content:

    {
    "Logging": {
    "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
    }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
    "Default": "Host=localhost:5438; Database=TheBooksApp; Username=postgres; Password=postgres;"
    },
    "JwtSettings": {
    "Audience": "the-books-app-api-jwt-audience",
    "Issuer": "the-books-app-api-jwt-issuer",
    "TokenLiveTime": 24,
    "RefreshTokenLiveTime": 48,
    "Secret": "'hvjgq+_6565732635-2332&sxSt#j_!D.*k%_eO)eedriW?8JF,rZ9#<2?8NJUH}*?G"
    },
    "SwaggerGen": {
    "Title": "The Books App API",
    "Version": "v1",
    "Description": "The Books App API",
    "ContactName": "gheith alrawahi",
    "ContactEmail": "alrawahi.gheith@gmail.com",
    "LicenseName": "MIT",
    "LicenseUrl": "https://opensource.org/license/mit/"
    }
    }

Step 7: Create Model Classes
7.1. Inside the Model folder, create the AppUser class:

      using Microsoft.AspNetCore.Identity;
      
      namespace TheBooks.Api.Model;
      
      public class AppUser : IdentityUser
      {
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
      }

and create AppUserToken Class and AccessPlatformType enum

    using System.ComponentModel.DataAnnotations;  
    using Ghak.libraries.AppBase.Models;  
      
    namespace TheBooks.Api.Model;  
      
    public enum AccessPlatformType  
    {  
        Web,  
        Mobile,  
        Desktop,  
        Android,  
        Ios,  
    }  
      
    public class AppUserToken : BaseModel  
    {  
        [Required]  
        public string UserId { get; set; }  
        public AppUser User { get; set; }  
          
        public string? RefreshToken { get; set; }  
        public DateTime? RefreshTokenExpires { get; set; }  
        public bool IsExpired => DateTime.UtcNow >= RefreshTokenExpires;  
        public AccessPlatformType Platform { get; set; } = AccessPlatformType.Web;  
    }

7.2. Inside the Data folder, create the AppDbContext class:

    using Ghak.libraries.AppBase.Extensions;  
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;  
    using Microsoft.EntityFrameworkCore;  
    using TheBooks.Api.Model;  
      
    namespace TheBooks.Api.Data;  
      
    public class AppDbContext: IdentityDbContext<AppUser>  
    {  
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)  
        {  
         }  
           
     public DbSet<AppUserToken> AppUserTokens { get; set; }  
          
        protected override void OnModelCreating(ModelBuilder modelBuilder)  
        {  
            modelBuilder.ActivateModelSoftDelete();  
            base.OnModelCreating(modelBuilder);  
        }  
    }

7.3. Also inside the Data folder, create the DataSeeder class:

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

Step 8: Create Startup Configuration
8.1. Inside the Startup folder, create the following files:

    AppRegistrations.cs
    AppSettings.cs
    ServicesSettings.cs
    
    8.2. Replace the code inside each file according to your existing instructions.
    
    using TheBooks.Api.Repositories.Auth;  
      
    namespace TheBooks.Api.Startup;  
      
    public static class AppRegistrations  
    {  
        public static IServiceCollection RepositoriesRegistration(this IServiceCollection services)  
        {  
             
            return services;  
        }  
          
        public static IServiceCollection ServicesRegistration(this IServiceCollection services)  
        {  
      
            return services;  
        }  
    }


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

Step 9: Update Program.cs
9.1. Replace the existing code inside Program.cs with the provided code in your original instructions.

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

Step 10: Database Migration and Update
10.1. Run the following commands to initialize the database:


    dotnet ef migrations add "Init DB"
    dotnet ef database update

Step 11: Add Authentication

11.1. Create the Auth DTOs inside the Dto/Auth folder.


AuthUserDto.cs

    public class AuthUserDto  
    {  
        public string Id { get; set; }  
        public string FullName { get; set; }  
        public string UserName { get; set; }  
        public bool IsActive { get; set; }  
        public List<string> Roles { get; set; }  
        public DateTime createdAt { get; set; }  
        public DateTime updatedAt { get; set; }  
    }

ResetPasswordDto.cs

    using System.ComponentModel.DataAnnotations;  
      
    namespace TheBooks.Api.Dto.Auth;  
      
    public class ResetPasswordDto  
    {  
        [Required]  
        public string Identifier { get; set; }  
        [Required]  
        public string NewPassword { get; set; }  
    }

UserLoginDto.cs

    using System.ComponentModel.DataAnnotations;  
    using TheBooks.Api.Model;  
      
    namespace TheBooks.Api.Dto.Auth;  
      
    public class UserLoginDto  
    {  
        [Required]  
        public string Identifier { get; set; }  
        [Required]  
        public string Password { get; set; }  
        public AccessPlatformType Platform { get; set; } = AccessPlatformType.Web;  
    }


UserRegisterDto.cs

    using System.ComponentModel.DataAnnotations;  
      
    namespace TheBooks.Api.Dto.Auth;  
      
    public class UserRegisterDto  
    {  
        [Required]  
        public string Username { get; set; }  
        [Required]  
        public string Phone { get; set; }  
        [Required]  
        public string Email { get; set; }  
        [Required]  
        public string Password { get; set; }  
    }

11.2. Inside the Repositories/Auth folder, create the IAuthRepository interface and AuthRepository implementation.

IAuthRepository .cs

    using Ghak.libraries.AppBase.DTO;  
    using Ghak.libraries.AppBase.Models;  
    using TheBooks.Api.Dto.Auth;  
      
    namespace TheBooks.Api.Repositories.Auth;  
      
    public interface IAuthRepository  
    {  
        Task<ApiResponse<bool>> Register(UserRegisterDto request);  
        Task<ApiResponse<LoginResponseDto>> Login(UserLoginDto request);  
        Task<ApiResponse<LoginResponseDto>> RefreshLogin(string refreshToken);  
        Task<ApiResponse<bool>> ResetPassword(ResetPasswordDto request);  
    }

AuthRepository.cs

    using System.IdentityModel.Tokens.Jwt;  
    using System.Security.Claims;  
    using Ghak.libraries.AppBase.DTO;  
    using Ghak.libraries.AppBase.Exceptions;  
    using Ghak.libraries.AppBase.Extensions;  
    using Ghak.libraries.AppBase.Models;  
    using Microsoft.AspNetCore.Identity;  
    using Microsoft.EntityFrameworkCore;  
    using TheBooks.Api.Data;  
    using TheBooks.Api.Dto.Auth;  
    using TheBooks.Api.Model;  
      
    namespace TheBooks.Api.Repositories.Auth;  
      
    public class AuthRepository(AppDbContext context,  
        IHostEnvironment hostingEnvironment,  
        UserManager<AppUser> userManager,  
        IHttpContextAccessor httpContextAccessor,  
        SignInManager<AppUser> signInManager) : IAuthRepository  
    {  
        public async Task<ApiResponse<bool>> Register(UserRegisterDto request)  
        {  
            var response = new ApiResponse<bool>();  
            try  
      {  
                var user = new AppUser()  
      {  
                    UserName = request.Username,  
                    Email = request.Email,  
                    PhoneNumber = request.Phone,  
      };  
      
                var result = await userManager.CreateAsync(user, request.Password);  
      
                if (!result.Succeeded)  
                {  
                    response.Errors = result.Errors.ToDictionary(k => k.Code, v => v.Description);  
                    return response;  
                }  
      
                response.Data = true;  
                return response;  
            }  
            catch (AppException exception)  
            {  
                response.StatusCode = exception.ErrorCode;  
                response.Errors.Add(exception.ErrorTitle, exception.Message);  
            }  
            catch (Exception exception)  
            {  
                if (!hostingEnvironment.IsProduction())  
                {  
                    response.StatusCode = 500;  
                    response.Errors.Add("server error", exception.Message);  
                }  
      
                Console.WriteLine($"Error, Message {exception.Message}");  
            }  
      
            return response;  
        }  
      
        public async Task<ApiResponse<LoginResponseDto>> Login(UserLoginDto request)  
        {  
            var response = new ApiResponse<LoginResponseDto>();  
            try  
      {  
                var user = await CheckLoginUser(request.Identifier);  
      
                if (string.IsNullOrEmpty(user.PasswordHash))  
                    throw new AppException("user password is not set", 101);  
                  
                var result = userManager.PasswordHasher.VerifyHashedPassword(user,  
                    user.PasswordHash,  
                    request.Password);  
      
                if (result != PasswordVerificationResult.Success)  
                    throw new AppException("user info is not correct", 101);  
      
                return await CreateLoginResponse(user, "api-login", request.Platform, response);  
            }  
            catch (AppException exception)  
            {  
                response.StatusCode = exception.ErrorCode;  
                response.Errors.Add(exception.ErrorTitle, exception.Message);  
            }  
            catch (Exception exception)  
            {  
                if (!hostingEnvironment.IsProduction())  
                {  
                    response.StatusCode = 500;  
                    response.Errors.Add("server error", exception.Message);  
                }  
      
                Console.WriteLine($"Error, Message {exception.Message}");  
            }  
      
            return response;  
        }  
      
      
        public async Task<ApiResponse<LoginResponseDto>> RefreshLogin(string refreshToken)  
        {  
            var response = new ApiResponse<LoginResponseDto>();  
            try  
      {  
                var principal = httpContextAccessor?.HttpContext!.User;  
                if (principal == null)  
                    throw new AppException("user login is not valid", 101);  
      
                var user = await userManager.GetUserAsync(principal);  
                if (user == null)  
                    throw new AppException("User not found", 101);  
      
                var userToken = await context.AppUserTokens  
      .Include(r => r.User)  
                    .FirstOrDefaultAsync(r => r.IsActive &&  
                                              r.RefreshToken == refreshToken &&  
                                              r.UserId == user.Id);  
      
                if (userToken == null)  
                    throw new AppException("refresh token is not correct", 101);  
      
                var error = "";  
                if (userToken.IsExpired)  
                {  
                    error = "refresh token is expired";  
                }  
      
                if (!userToken.User.IsActive)  
                    error = "user account is disabled";  
      
                userToken.DeleteSoftly();  
                await context.SaveChangesAsync();  
      
                if (!string.IsNullOrEmpty(error))  
                    throw new AppException(error, 101);  
      
                return await CreateLoginResponse(user, "refresh", userToken.Platform, response);  
            }  
            catch (AppException exception)  
            {  
                response.StatusCode = exception.ErrorCode;  
                response.Errors.Add(exception.ErrorTitle, exception.Message);  
            }  
            catch (Exception exception)  
            {  
                if (!hostingEnvironment.IsProduction())  
                {  
                    response.StatusCode = 500;  
                    response.Errors.Add("server error", exception.Message);  
                }  
      
                Console.WriteLine($"Error, Message {exception.Message}");  
            }  
      
            return response;  
        }  
      
        private async Task<ApiResponse<LoginResponseDto>> CreateLoginResponse(AppUser user,  
            string method,  
            AccessPlatformType platformType, ApiResponse<LoginResponseDto> response)  
        {  
            try  
      {  
                await signInManager.SignInAsync(user, false,   
                    method);  
      
                var identityClaims = new ClaimsIdentityOptions();  
                var authClaims = new List<Claim>  
                {  
                    new("Id", user.Id),  
                    new(JwtRegisteredClaimNames.Sub, user.Id),  
                    new(JwtRegisteredClaimNames.Jti, user.Id),  
                    new(identityClaims.UserIdClaimType, user.Id),  
                    new(identityClaims.EmailClaimType, user.Email ?? ""),  
                    new(ClaimTypes.Name, user.UserName ?? "-"),  
                    new(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),  
      };  
      
                var userRoles = await userManager.GetRolesAsync(user);  
                authClaims.AddRange(  
                    userRoles.Select(userRole =>  
                        new Claim(identityClaims.RoleClaimType, userRole)));  
      
                response.Data = AuthenticationSetting.UserLogin(authClaims);  
      
                await context.AppUserTokens.AddAsync(new AppUserToken()  
      {  
                    UserId = user.Id,  
                    RefreshToken = response.Data.RefreshToken,  
                    RefreshTokenExpires = response.Data.RefreshTokenExpiredAt,  
                    Platform = platformType  
      });  
                await context.SaveChangesAsync();  
      
                return response;  
            }  
            catch (Exception e)  
            {  
                Console.WriteLine(e);  
                throw;  
            }  
        }  
      
      
        public async Task<ApiResponse<bool>> ResetPassword(ResetPasswordDto request)  
        {  
            var response = new ApiResponse<bool>();  
            try  
      {  
                var user = await CheckLoginUser(request.Identifier);  
      
                var code = Guid.NewGuid().ToString("N");  
                var result = await userManager.ResetPasswordAsync(user, code, request.NewPassword);  
      
                if (!result.Succeeded)  
                {  
                    response.Errors = result.Errors.ToDictionary(k => k.Code, v => v.Description);  
                    return response;  
                }  
      
                response.Data = true;  
                return response;  
            }  
            catch (AppException exception)  
            {  
                response.StatusCode = exception.ErrorCode;  
                response.Errors.Add(exception.ErrorTitle, exception.Message);  
            }  
            catch (Exception exception)  
            {  
                if (!hostingEnvironment.IsProduction())  
                {  
                    response.StatusCode = 500;  
                    response.Errors.Add("server error", exception.Message);  
                }  
      
                Console.WriteLine($"Error, Message {exception.Message}");  
            }  
      
            return response;  
        }  
      
        private async Task<AppUser> CheckLoginUser(string identifier)  
        {  
            identifier = identifier.Trim().TrimStart('@');  
            var user = await context.Users  
      .FirstOrDefaultAsync(r =>  
                    (r.PhoneNumber != null  
      && (identifier.Contains(r.PhoneNumber))  
      || r.Email == identifier  
      || r.UserName == identifier)  
      );  
      
            if (user == null) throw new AppException("this user dose not exist", 101);  
      
            if (!user.IsActive) throw new AppException("this user account is disabled", 101);  
      
            return user;  
        }  
    }

11.3. Register the IAuthRepository in AppRegistrations.cs.

    public static IServiceCollection RepositoriesRegistration(this IServiceCollection services)  
    {  
        services.AddScoped<IAuthRepository, AuthRepository>();  
        return services;  
    }

11.4. Create the AuthController inside the Controllers folder, including endpoints for user registration, login, refresh login, and password reset.

    using Ghak.libraries.AppBase.DTO;  
    using Ghak.libraries.AppBase.Models;  
    using Microsoft.AspNetCore.Authorization;  
    using Microsoft.AspNetCore.Mvc;  
    using TheBooks.Api.Dto.Auth;  
    using TheBooks.Api.Repositories.Auth;  
      
    namespace TheBooks.Api.Controllers;  
      
    [Route("api/[controller]")]  
    public class AuthController(IAuthRepository repository)  
        : ControllerBase  
    {  
        [HttpPost("user-register")]  
        public async Task<ActionResult<ApiResponse<bool>>> Register(UserRegisterDto request)  
        {  
            return await repository.Register(request);  
        }  
          
        [HttpPost("login")]  
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(UserLoginDto request)  
        {  
            return await repository.Login(request);  
        }  
          
        [Authorize]  
        [HttpPost("refresh-login")]  
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshLogin(string refreshToken)  
        {  
            return await repository.RefreshLogin(refreshToken);  
        }  
          
          
        [HttpPost("reset-password")]  
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword( ResetPasswordDto request)  
        {  
            return await repository.ResetPassword(request);  
        }  
          
          
    }

now you can test the api using swagger

```
dotnet watch
```

you should get the following endpoints
- /api/Auth/user-register
- /api/Auth/login
- /api/Auth/refresh-login
- /api/Auth/reset-password

Step 12: Add Roles

inside helper folder create AppRoles.cs

```
namespace TheBooks.Api.Helpers;

public class AppUsersRoles
{
    public const string Root = "Root";
    public const string Admin = "Admin";
    public const string User = "User";
    
    public static List<string> GetRolesList()
    {
        return new List<string>
        {
            Root,
            Admin,
            User,
        };
    }
}
```

create AssignRolesToUserDto inside Dto/Auth

```
using System.ComponentModel.DataAnnotations;

namespace TheBooks.Api.Dto.Auth;

public class AssignRolesToUserDto
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public List<string> Roles { get; set; }
}
```

add new method to IAuthRepository
```
Task<ApiResponse<bool>> AssignRoles(AssignRolesToUserDto request);
```

and implement it in AuthRepository
```
public async Task<ApiResponse<bool>> AssignRoles(AssignRolesToUserDto request)
    {
        var response = new ApiResponse<bool>();
        try
        {
            var user = await context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                throw new AppException("User is not found", 101);
            }

            var roles = AppUsersRoles.GetRolesList();
            var correctRoles = request.Roles
                .Where(r => roles.Contains(r.Humanize(LetterCasing.Title)))
                .ToList();

            var oldRoles = (await userManager.GetRolesAsync(user)).ToList();
            correctRoles = correctRoles.Except(oldRoles).ToList();
            var result = await userManager.AddToRolesAsync(user, correctRoles);

            if (result.Succeeded)
            {
                response.Data = true;
                return response;
            }

            response.Errors = result.Errors.ToDictionary(k => k.Code, v => v.Description);

            return response;
        }
        catch (AppException exception)
        {
            response.StatusCode = exception.ErrorCode;
            response.Errors.Add(exception.ErrorTitle, exception.Message);
        }
        catch (Exception exception)
        {
            if (!hostingEnvironment.IsProduction())
            {
                response.StatusCode = 500;
                response.Errors.Add("server error", exception.Message);
            }

            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }
```

add new endpoint to AuthController
```
    [HttpPost( "assign-roles-to-user")]
    [Authorize(Roles = $"{AppUsersRoles.Root}|{AppUsersRoles.Root}")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoles([FromBody]  AssignRolesToUserDto request)
    {
        return await repository.AssignRoles(request);
    }
```

before we check the endpoint we need to add roles to the database and default 
user with root role we write the following code in DataSeeder.cs

```
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
```

now you can test the api using swagger

```
dotnet watch
```


