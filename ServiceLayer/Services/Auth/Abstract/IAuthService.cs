using EntityLayer.DTOs.Auth;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.Auth.Abstract
{
	public interface IAuthService
    {
        Task<GeneralResponse> RegisterUserAsync(UserDTO model);
        Task<LoginResponse> LoginAsync(LoginDTO model);
        Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordDTO model);
        Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDTO model);
    }
}
