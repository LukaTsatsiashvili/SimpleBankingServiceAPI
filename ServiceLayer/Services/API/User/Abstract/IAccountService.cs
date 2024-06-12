using EntityLayer.DTOs.User.Account;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
    public interface IAccountService
	{
		Task<List<AccountDTO>> GetAllAccountAsync();
		Task<AccountDTO> GetAccountByIdAsync(string userId);
		Task<GeneralResponse> CreateAccountAsync(string userId);
		Task RemoveAccountAsync(string userId);

	}
}
