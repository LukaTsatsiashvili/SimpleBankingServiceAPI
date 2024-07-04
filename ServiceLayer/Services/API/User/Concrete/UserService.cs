using AutoMapper;
using EntityLayer.DTOs.Auth;
using EntityLayer.DTOs.Image;
using EntityLayer.DTOs.User;
using EntityLayer.Entities.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Services.API.User.Abstract;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.API.User.Concrete
{
	public class UserService(
		UserManager<AppUser> userManager,
		IWebHostEnvironment environment,
		IHttpContextAccessor httpContextAccessor,
		IMapper mapper) : IUserService
	{
		public async Task<ProfilePictureUploadResponse> UploadProfilePictureAsync(string userId, ImageUploadDTO model)
		{
			if (model.File is null || model.File.Length == 0 || userId is null)
				return new ProfilePictureUploadResponse(false, null!, "Failed to upload image!");

			// Find the user by the provided user ID 
			var user = await userManager.FindByIdAsync(userId);
			if (user is null) return new ProfilePictureUploadResponse(false, null!, "User not found!");

			// Create the folder to store the uploaded images, if it doesn't exist
			var uploadsFolder = Path.Combine(environment.ContentRootPath, "Uploads", $"{user.Email}");
			Directory.CreateDirectory(uploadsFolder);

			// Generate a unique file name for the uploaded image
			var uniqueFileName = model.FileName + Path.GetExtension(model.File.FileName);

			// Combine the uploads folder path with the unique file name to generate the full file path
			var filePath = Path.Combine(uploadsFolder, uniqueFileName);

			// Check if the user has an existing profile picture
			var currentProfilePicture = user.ProfileImagePath;
			if (!string.IsNullOrEmpty(currentProfilePicture))
			{
				// Construct the path to the existing profile picture and delete it
				var imagePath = $"{environment.ContentRootPath}/Uploads/{user.Email}/{Path.GetFileName(currentProfilePicture)}";
				File.Delete(imagePath);
			}

			// Save the uploaded image to the designated directory
			using var fileStream = new FileStream(filePath, FileMode.Create);
			await model.File.CopyToAsync(fileStream);

			// Update the user's profile picture path with the URL to the uploaded image
			user.ProfileImagePath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}/Uploads/{user.Email}/{uniqueFileName}";
			await userManager.UpdateAsync(user);

			return new ProfilePictureUploadResponse(true, user.ProfileImagePath, "Image uploaded successfully!");
		}

		public async Task<GetUserInformationResponse> GetUserInformationAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId)) return new GetUserInformationResponse(false, "Id must be provided!", null);

			var user = await userManager
				.Users
				.Include(x => x.Accounts)
				.FirstOrDefaultAsync(x => x.Id == userId);

			if (user is null) return new GetUserInformationResponse(false, "Unauthorized!", null);

			var mappedUser = mapper.Map<UserInformationDTO>(user);
			if (mappedUser is null)
				return new GetUserInformationResponse(false, "Something went wrong. Please try again later!", null); 

			return new GetUserInformationResponse(true, "User found successfully!", mappedUser);
		}

		public async Task<GeneralResponse> RemoveProfilePictureAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId)) return new GeneralResponse(false, "Unable to remove picture!");

			var user = await userManager.FindByIdAsync(userId);
			if (user is null) return new GeneralResponse(false, "User not found!");

			var profilePicture = user.ProfileImagePath;
			if (string.IsNullOrEmpty(profilePicture)) return new GeneralResponse(false, "Picture does not exist!");

			var picturePath = $"{environment.ContentRootPath}/Uploads/{user.Email}/{Path.GetFileName(profilePicture)}";
			if (File.Exists(picturePath))
			{
				File.Delete(picturePath);
			};

			user.ProfileImagePath = null;
			var result = await userManager.UpdateAsync(user);
			if (!result.Succeeded) return new GeneralResponse(false, "Failed to delete picture!");

			return new GeneralResponse(true, "Picture deleted successfully");
		}

		public async Task<GeneralResponse> ChangePasswordAsync(string userId, ChangePasswordDTO model)
		{
			if (string.IsNullOrEmpty(userId) || model is null) 
				return new GeneralResponse(false, "Unable to change password!");

			var user = await userManager.FindByIdAsync(userId);
			if (user is null) return new GeneralResponse(false, "User not found!");

			var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
			if (!result.Succeeded) return new GeneralResponse(false, "Failed to change password!");

			return new GeneralResponse(true, "Password changed successfully!");
		}

		public async Task<GeneralResponse> DeleteUserAsync(string userId)
		{
			if (string.IsNullOrEmpty(userId)) return new GeneralResponse(false, "Invalid request!");

			var user = await userManager.FindByIdAsync(userId);
			if (user is null) return new GeneralResponse(false, "Invalid request!");

			var profilePicture = user.ProfileImagePath;
			if (!string.IsNullOrEmpty(profilePicture))
			{
				var imagePath = $"{environment.ContentRootPath}/Uploads/{user.Email}/{Path.GetFileName(profilePicture)}";
				File.Delete(imagePath);
			}

			var result = await userManager.DeleteAsync(user);
			if (!result.Succeeded) return new GeneralResponse(false, "Failed to delete account!");

			return new GeneralResponse(true, "Account deleted successfully");

		}

	}
}
