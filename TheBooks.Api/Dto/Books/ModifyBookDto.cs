using Ghak.libraries.AppBase.DTO;
using Humanizer;
using TheBooks.Api.Model;

namespace TheBooks.Api.Dto.Books;

public class ModifyBookDto
{
    public string? Id { get; set; }
    public string? OwnerId { get; set; }
    public string? BookCollectionId { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }
    public int ReleaseYear { get; set; }
    public int PublishYear { get; set; }
    public string? ISBN { get; set; }
    public string? Language { get; set; }
    public string? Publisher { get; set; }
    public List<BookType> Types { get; set; }
}