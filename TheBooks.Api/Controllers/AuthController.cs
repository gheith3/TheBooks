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