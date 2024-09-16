using EntityLayer.DTOs;
using EntityLayer.DTOs.User;
using Microsoft.AspNetCore.Http;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
	public interface IAdminService
	{
		Task<GetAllUsersResponse> GetAllUsersAsync();
		Task<GetSingleUserResponse> GetSingleUserAsync(Guid id);
		Task<GetTransactionHistoryResponse> GetUserTransactionsAsync(Guid id);
		Task<GeneralResponse> CreateUserAsync(CreateUserDTO model);
		Task<GeneralResponse> UpdateUserInformationAsync(Guid id, UpdateUsersInformationDTO model);

		Task<GenerateExcelFileResponse> GenerateUserExcelFileAsync(Guid? id);
		Task<GenerateExcelFileResponse> GenerateAuditLogsExcelFileAsync(string? userEmail);
		Task<GeneralResponse> ImportMonthlyReportFileToDBAsync(MonthlyReportDTO model);
	}
}
