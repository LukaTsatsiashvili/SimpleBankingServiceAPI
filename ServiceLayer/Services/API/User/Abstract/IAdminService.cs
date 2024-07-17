using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
	public interface IAdminService
	{
		Task<GetAllUsersResponse> GetAllUsersAsync();
		Task<GetSingleUserResponse> GetSingleUserAsync();
	}
}
