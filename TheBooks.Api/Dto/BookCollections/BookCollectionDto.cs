using Ghak.libraries.AppBase.DTO;

namespace TheBooks.Api.Dto.BookCollections;

public class BookCollectionDto : BaseDto
{
    public string OwnerId { get; set; }
    public string Owner { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
}