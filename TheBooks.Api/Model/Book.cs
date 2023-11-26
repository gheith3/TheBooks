using System.ComponentModel.DataAnnotations;
using Ghak.libraries.AppBase.Models;

namespace TheBooks.Api.Model;

public enum BookType
{
    History,
    Novel,
    Science,
    Fantasy,
    Biography,
    Other,
}

public class Book : BaseModel
{
    [Required] 
    public AppUser Owner { get; set; }
    public string OwnerId { get; set; }
    
    public BookCollection? BookCollection { get; set; }
    public string? BookCollectionId { get; set; }
    
    [Required] public string Title { get; set; }
    public string? Description { get; set; }
    public int ReleaseYear { get; set; } = DateTime.Now.Year;
    public int PublishYear { get; set; } = DateTime.Now.Year;
    public string? ISBN { get; set; }
    public string? Language { get; set; }
    public string? Publisher { get; set; }
    public List<BookType> Types { get; set; } = new();
}