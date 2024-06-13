using AutoMapper;
using EntityLayer.DTOs.Transaction;
using EntityLayer.Entities.User;


namespace ServiceLayer.Mapping
{
	public class TransactionMapping : Profile
	{
        public TransactionMapping()
        {
            CreateMap<TransactionCreateDTO, Transaction>()
                .ForMember(x => x.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(x => x.RecipientAccountNumber, opt => opt.MapFrom(src => src.RecipientAccountNumber))
                .ReverseMap();
        }
    }
}
