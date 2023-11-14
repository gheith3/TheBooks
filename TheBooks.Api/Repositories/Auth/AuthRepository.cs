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