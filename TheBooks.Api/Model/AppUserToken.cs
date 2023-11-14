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