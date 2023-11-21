using System.ComponentModel.DataAnnotations;
namespace TheBooks.Api.Dto.BookCollections;

public class ModifyBookCollectionDto 
{
    public string? Id { get; set; }
    public string? OwnerId { get; set; }
    [Required]
    public string Title { get; set; }
    public string? Description { get; set; }
}