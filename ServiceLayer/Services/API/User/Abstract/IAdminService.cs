using EntityLayer.DTOs.User;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
	public interface IAdminService
	{
		Task<GetAllUsersResponse> GetAllUsersAsync();
		Task<GetSingleUserResponse> GetSingleUserAsync(Guid id);
		Task<GetTransactionHistoryResponse> GetUserTransactionsAsync(Guid id);
		Task<GeneralResponse> UpdateUserInformationAsync(Guid id, UpdateUsersInformationDTO model);

		Task<GenerateUserExcelFileResponse> GenerateUserExcelFileAsync(Guid? id);
	}
}
