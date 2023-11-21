using Ghak.libraries.AppBase.Interfaces;
using TheBooks.Api.Dto.BookCollections;
using TheBooks.Api.Model;

namespace TheBooks.Api.Repositories.BookCollections;

public interface IBookCollectionsRepository
    : ICrudRepository<string, BookCollection, BookCollectionDto, ModifyBookCollectionDto, ToModifyBookCollectionDto>
{
}