using EntityLayer.DTOs.Auth;
using EntityLayer.DTOs.Image;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
	public interface IUserService
	{
		Task<ProfilePictureUploadResponse> UploadProfilePictureAsync(string userId, ImageUploadDTO model);
		Task<GeneralResponse> RemoveProfilePictureAsync(string userId);
		Task<GeneralResponse> ChangePasswordAsync(string userId, ChangePasswordDTO model);
		Task<GeneralResponse> DeleteAccountAsync(string userId);
	}
}
