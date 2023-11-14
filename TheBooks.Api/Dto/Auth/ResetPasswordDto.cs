using System.ComponentModel.DataAnnotations;

namespace TheBooks.Api.Dto.Auth;

public class ResetPasswordDto
{
    [Required]
    public string Identifier { get; set; }
    [Required]
    public string NewPassword { get; set; }
}