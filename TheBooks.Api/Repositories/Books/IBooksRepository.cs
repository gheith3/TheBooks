using Ghak.libraries.AppBase.Interfaces;
using TheBooks.Api.Dto.Books;
using TheBooks.Api.Model;

namespace TheBooks.Api.Repositories.Books;

public interface IBooksRepository
    : ICrudRepository<string, Book, BookDto, ModifyBookDto, ToModifyBookDto>
{
}