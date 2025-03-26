using AutoMapper;
using ProjeApiModel.DTOs.Data;
using ProjeApiModel.DTOs.IdentityDTO;
using ProjeApiModel.ViewModel;
using ProjeApiModel.ViewModel.Identity;

namespace ProjeApiBusiness.Mapping_1
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDTO>()
        .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash));
            CreateMap<Category, CategoryDTO>().ReverseMap();

        }
    }
}
