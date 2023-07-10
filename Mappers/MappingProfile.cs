using AutoMapper;
using Movies.DataTransferObjects;
using Movies.Models;

namespace Movies.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserDto, User>()
                .ForMember(u => u.InterestedCategories, opt => opt.MapFrom(udto => udto.InterestedCategories))
                .ForMember(u => u.Comments, opt => opt.Ignore());

            CreateMap<CategoryDto, Category>();            
        }
    }
}
