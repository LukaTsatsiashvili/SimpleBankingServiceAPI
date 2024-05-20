using EntityLayer.DTOs.Image;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Abstract
{
	public interface IUserService
	{
		Task<ProfilePictureUploadResponse> UploadProfilePictureAsync(string userId, ImageUploadDTO model);
		Task<GeneralResponse> DeleteAccountAsync(string userId);
		Task<GeneralResponse> RemoveProfilePictureAsync(string userId);
	}
}
