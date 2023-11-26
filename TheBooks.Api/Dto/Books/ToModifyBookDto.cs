using Ghak.libraries.AppBase.DTO;
using Ghak.libraries.AppBase.Models;
using Humanizer;
using TheBooks.Api.Model;

namespace TheBooks.Api.Dto.Books;

public class ToModifyBookDto : BaseToModifyDto<ModifyBookDto>
{
    public List<ListItem<int>> Types { get; set; }
}