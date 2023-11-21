using AutoMapper;
using TheBooks.Api.Dto.Auth;
using TheBooks.Api.Model;

namespace TheBooks.Api.Mappers;

public class AuthMapperProfile : Profile
{
    public AuthMapperProfile()
    {
        CreateMap<AppUser, AuthUserDto>();
    }
}