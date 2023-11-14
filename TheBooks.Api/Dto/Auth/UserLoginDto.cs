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