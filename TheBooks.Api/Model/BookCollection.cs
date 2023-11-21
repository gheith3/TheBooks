using System.ComponentModel.DataAnnotations;
using Ghak.libraries.AppBase.Models;

namespace TheBooks.Api.Model;

public class BookCollection : BaseModel
{
    [Required]
    public string OwnerId { get; set; }
    public  AppUser Owner { get; set; }
    
    [Required] 
    public string Title { get; set; }
    public string? Description { get; set; }
}