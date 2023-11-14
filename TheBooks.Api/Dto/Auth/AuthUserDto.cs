namespace TheBooks.Api.Dto.Auth;

public class AuthUserDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}