using Ghak.libraries.AppBase.Models;
using TheBooks.Api.Model;

namespace TheBooks.Api.Services.Auth;

public interface IAuthUserServices
{
    Task<AppUser> AuthUser();
    Task<List<string>> Roles();
    Task<bool> IsInRoles(string checkRoles);
    Task<string> Id();
}