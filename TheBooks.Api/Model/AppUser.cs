using Microsoft.AspNetCore.Identity;

namespace TheBooks.Api.Model;

public class AppUser : IdentityUser
{
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public virtual List<AppUserToken> Tokens { get; set; }
}