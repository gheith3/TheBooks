using AutoMapper;
using TheBooks.Api.Dto.Books;
using TheBooks.Api.Model;

namespace TheBooks.Api.Mappers;

public class BookMapper : Profile
{
    public BookMapper()
    {
        BookMappers();
    }

    private void BookMappers()
    {
        CreateMap<Book, BookDto>();
        CreateMap<ModifyBookDto, Book>()
            .ReverseMap();
    }
}