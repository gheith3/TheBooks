using AutoMapper;
using Ghak.libraries.AppBase.DTO;
using Ghak.libraries.AppBase.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TheBooks.Api.Dto.Auth;
using TheBooks.Api.Helpers;
using TheBooks.Api.Model;
using TheBooks.Api.Repositories.Auth;

namespace TheBooks.Api.Controllers;

[Route("api/[controller]")]
public class AuthController(UserManager<AppUser> userManager, 
        IAuthRepository repository, IMapper mapper)
    : ControllerBase
{
    [HttpPost("user-register")]
    public async Task<ActionResult<ApiResponse<bool>>> Register([FromBody] UserRegisterDto request)
    {
        return await repository.Register(request);
    }
    
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] UserLoginDto request)
    {
        return await repository.Login(request);
    }
    
    [HttpPost( "get-auth-user")]
    [Authorize]
    public async Task<ActionResult<AuthUserDto>> GetUser()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        var authUser = mapper.Map<AuthUserDto>(user);
        var roles = await userManager.GetRolesAsync(user);
        authUser.Roles = roles.ToList();
        return Ok(authUser);
    }

    
    [Authorize]
    [HttpPost("refresh-login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RefreshLogin([FromBody] string refreshToken)
    {
        return await repository.RefreshLogin(refreshToken);
    }
    
    
    [HttpPost("reset-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto request)
    {
        return await repository.ResetPassword(request);
    }
    
    [HttpPost( "assign-roles-to-user")]
    [Authorize(Roles = $"{AppUsersRoles.Root}|{AppUsersRoles.Root}")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRoles([FromBody]  AssignRolesToUserDto request)
    {
        return await repository.AssignRoles(request);
    }
    
}