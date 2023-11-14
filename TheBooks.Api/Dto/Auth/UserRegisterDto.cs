using System.ComponentModel.DataAnnotations;

namespace TheBooks.Api.Dto.Auth;

public class UserRegisterDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}