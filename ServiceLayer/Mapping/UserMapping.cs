using AutoMapper;
using EntityLayer.DTOs.User;
using EntityLayer.Entities.Auth;

namespace ServiceLayer.Mapping
{
	public class UserMapping : Profile
	{
        public UserMapping()
        {
            CreateMap<AppUser, UserInformationDTO>()
                .ForMember(dest => dest.Account, src => src.MapFrom(x => x.Accounts))
                .ReverseMap();
        }
    }
}
