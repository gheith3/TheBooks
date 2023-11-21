using Ghak.libraries.AppBase.Exceptions;
using Microsoft.AspNetCore.Identity;
using TheBooks.Api.Model;

namespace TheBooks.Api.Services.Auth;

public class AuthUserServices(IHostEnvironment hostingEnvironment,
    UserManager<AppUser> userManager,
    IHttpContextAccessor httpContextAccessor) : IAuthUserServices
{
    public async Task<AppUser> AuthUser()
    {
        var response = new AppUser();
        try
        {
            var principal = httpContextAccessor?.HttpContext!.User;
            if (principal == null)
                throw new AppException("user login is not valid", 101);

            var user = await userManager.GetUserAsync(principal);
            if (user == null)
                throw new AppException("User not found", 101);
            
            return user;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<List<string>> Roles()
    {
        var response = new List<string>();
        try
        {
            var principal = httpContextAccessor?.HttpContext!.User;
            if (principal == null)
                throw new AppException("user login is not valid", 101);

            var user = await userManager.GetUserAsync(principal);
            if (user == null)
                throw new AppException("User not found", 101);
            return (await userManager.GetRolesAsync(user)).ToList();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }

    public async Task<bool> IsInRoles(string checkRoles)
    {
        var response = false;
        try
        {
            var roles = await Roles();
            if (!roles.Any())
                return response;
            
            checkRoles = checkRoles
                .Replace(",", "|")
                .Replace(";", "|")
                .Replace(":", "|");
            
            var checkRolesList = checkRoles.Split("|").ToList();
            
            response = roles.Any(r => checkRolesList.Contains(r));
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return response;
    }
    
    public async Task<string> Id()
    {
        try
        {
            var user = await AuthUser();
            return user.Id;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Error, Message {exception.Message}");
        }

        return string.Empty;
    }
}