using EntityLayer.DTOs;
using static EntityLayer.DTOs.ServiceResponses;

namespace ServiceLayer.Services.Abstract
{
	public interface IUserService
	{
		Task<GeneralResponse> RegisterUserAsync(UserDTO model);
		Task<LoginResponse> LoginAsync(LoginDTO model);
		Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordDTO model);
		Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDTO model);
	}
}
