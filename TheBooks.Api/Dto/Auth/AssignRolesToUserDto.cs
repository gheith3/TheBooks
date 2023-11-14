using System.ComponentModel.DataAnnotations;

namespace TheBooks.Api.Dto.Auth;

public class AssignRolesToUserDto
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public List<string> Roles { get; set; }
}