using EntityLayer.DTOs.User.Account;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
    public interface IAccountService
	{
		Task<AccountResponse> GetAccountByIdAsync(Guid id);
		Task<GeneralResponse> CreateAccountAsync(string userId);
		Task<GeneralResponse> RemoveAccountAsync(string userId, Guid id);

	}
}
