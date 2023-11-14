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