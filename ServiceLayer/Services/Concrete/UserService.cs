using EntityLayer.DTOs.Auth;
using EntityLayer.DTOs.Image;
using EntityLayer.Entities.Auth;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceLayer.Helpers;
using ServiceLayer.Services.Abstract;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Services.Concrete
{
    public class UserService(
		UserManager<AppUser> userManager,
		RoleManager<IdentityRole> roleManager,
		IConfiguration config,
		IEmailSendMethod sendEmailMethod,
		IWebHostEnvironment environment,
		IHttpContextAccessor httpContextAccessor
		) : IUserService
	{

		public async Task<LoginResponse> LoginAsync(LoginDTO model)
		{
			if (model == null) return new LoginResponse(false, null!, "Model is empty!");

			var getUser = await userManager.FindByEmailAsync(model.Email);
			if (getUser == null) return new LoginResponse(false, null!, "User not found!");

			bool checkUserPassword = await userManager.CheckPasswordAsync(getUser, model.Password);
			if (!checkUserPassword) return new LoginResponse(false, null!, "Email or Password is incorrect!");

			var getUserRole = await userManager.GetRolesAsync(getUser);
			var userSession = new UserSession(getUser.Id, getUser.UserName, getUser.Email, getUserRole.First());
			string token = GenerateJwtToken(userSession);

			return new LoginResponse(true, token, "Login completed!");
		}

		public async Task<GeneralResponse> RegisterUserAsync(UserDTO model)
		{

			if (model is null) return new GeneralResponse(false, "Model is empty!");
			var newUser = new AppUser()
			{
				Email = model.Email,
				PasswordHash = model.Password,
				UserName = model.Email
			};
			var user = await userManager.FindByEmailAsync(newUser.Email);
			if (user is not null) return new GeneralResponse(false, "User already registered!");

			var createUser = await userManager.CreateAsync(newUser, model.Password);
			if (!createUser.Succeeded) return new GeneralResponse(false, "Something went wrong! Please try again later.");

			// Assign default Role : 'Admin' to first registrar, rest is 'User'
			var checkAdmin = await roleManager.FindByNameAsync("Admin");
			if (checkAdmin is null)
			{
				await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
				await userManager.AddToRoleAsync(newUser, "Admin");
				return new GeneralResponse(true, "Account created!");
			}
			else
			{
				var checkUser = await roleManager.FindByNameAsync("User");
				if (checkUser is null)
				{
					await roleManager.CreateAsync(new IdentityRole() { Name = "User" });
				}

				await userManager.AddToRoleAsync(newUser, "User");
				return new GeneralResponse(true, "Account created!");
			}
		}

		private string GenerateJwtToken(UserSession session)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var userClaims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, session.Id!),
				new Claim(ClaimTypes.Name, session.Name!),
				new Claim(ClaimTypes.Email, session.Email!),
				new Claim(ClaimTypes.Role, session.Role!)
			};

			var token = new JwtSecurityToken(
				issuer: config["Jwt:Issuer"],
				audience: config["Jwt:Audience"],
				claims: userClaims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: credentials
				);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordDTO model)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user is null) return new ForgotPasswordResponse(false, "User not found!");

			var token = await userManager.GeneratePasswordResetTokenAsync(user);
			if (token is null) return new ForgotPasswordResponse(false, "Something went wrong! Please try again later.");

			// Sending token to user email
			await sendEmailMethod.SendPasswordResetToken(token, model.Email);

			return new ForgotPasswordResponse(true, "Token is sent to your Email! Please check your inbox.");
		}

		public async Task<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordDTO model)
		{
			var user = await userManager.FindByIdAsync(model.UserId);
			if (user is null) return new ResetPasswordResponse(false, "User not found!");

			var result = await userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
			if (!result.Succeeded) return new ResetPasswordResponse(false, "Password change failed!");

			return new ResetPasswordResponse(true, "Password changed successfully");
		}

		public async Task<ProfilePictureUploadResponse> ProfilePictureUploadAsync(string userId, ImageUploadDTO model)
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
	}
}
