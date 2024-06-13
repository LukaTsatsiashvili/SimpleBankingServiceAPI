using AutoMapper;
using EntityLayer.DTOs.Account;
using EntityLayer.Entities.User;

namespace ServiceLayer.Mapping
{
    public class AccountMapping : Profile
	{
        public AccountMapping()
        {
            CreateMap<Account, AccountDTO>().ReverseMap();
        }
    }
}
