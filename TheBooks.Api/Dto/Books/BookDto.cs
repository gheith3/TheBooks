using Ghak.libraries.AppBase.DTO;
using Humanizer;
using TheBooks.Api.Model;

namespace TheBooks.Api.Dto.Books;

public class BookDto : BaseDto
{
    public string Owner { get; set; }
    public string OwnerId { get; set; }

    public string? BookCollection { get; set; }
    public string? BookCollectionId { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }
    public int ReleaseYear { get; set; } = DateTime.Now.Year;
    public int PublishYear { get; set; } = DateTime.Now.Year;
    public string? ISBN { get; set; }
    public string? Language { get; set; }
    public string? Publisher { get; set; }
    public List<BookType> Types { get; set; } = new();

    public List<string> TypesTitle
    {
        get { return Types.Select(r => r.Humanize(LetterCasing.Title)).ToList(); }
    }
}