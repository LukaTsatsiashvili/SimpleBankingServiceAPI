using EntityLayer.DTOs.Transaction;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
	public interface ITransactionService
	{
		Task<TransactionResponse> CreateTransactionAsync(string senderId, TransactionCreateDTO model);
	}
}
