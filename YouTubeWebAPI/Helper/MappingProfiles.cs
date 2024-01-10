using AutoMapper;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Category,CategoryDto>().ReverseMap();
            CreateMap<Country,CountryDto>().ReverseMap();
            CreateMap<Owner,OwnerDto>().ReverseMap();
        }
    }
}
