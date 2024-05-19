using EntityLayer.DTOs.Auth;
using EntityLayer.DTOs.Image;
using Microsoft.AspNetCore.Http;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.Abstract
{
    public interface IUserService
	{
		Task<GeneralResponse> RegisterUserAsync(UserDTO model);
		Task<LoginResponse> LoginAsync(LoginDTO model);
		Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordDTO model);
		Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDTO model);
		Task<ProfilePictureUploadResponse> ProfilePictureUploadAsync(string userId, ImageUploadDTO model);
	}
}
