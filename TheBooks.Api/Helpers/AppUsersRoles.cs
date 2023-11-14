namespace TheBooks.Api.Helpers;

public class AppUsersRoles
{
    public const string Root = "Root";
    public const string Admin = "Admin";
    public const string User = "User";
    
    public static List<string> GetRolesList()
    {
        return new List<string>
        {
            Root,
            Admin,
            User,
        };
    }
}