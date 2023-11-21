using AutoMapper;
using TheBooks.Api.Dto.BookCollections;
using TheBooks.Api.Model;

namespace TheBooks.Api.Mappers;

public class BookCollectionMapper : Profile
{
    public BookCollectionMapper()
    {
        BookCollectionMappers();
    }

    private void BookCollectionMappers()
    {
        CreateMap<BookCollection, BookCollectionDto>();
        CreateMap<ModifyBookCollectionDto, BookCollection>()
            .ReverseMap();
    }
}