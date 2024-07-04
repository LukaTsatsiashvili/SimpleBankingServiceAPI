using AutoMapper;
using EntityLayer.DTOs.Account;
using EntityLayer.Entities.User;

namespace ServiceLayer.Mapping
{
    public class AccountMapping : Profile
	{
        public AccountMapping()
        {
            CreateMap<Account, AccountDTO>()
                .ForMember(x => x.SentTransactions, opt => opt.MapFrom(src => src.SentTransactions))
                .ForMember(x => x.ReceivedTransactions, opt => opt.MapFrom(src => src.ReceivedTransactions))
                .ReverseMap();
        }
    }
}
