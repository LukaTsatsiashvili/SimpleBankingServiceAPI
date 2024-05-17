using EntityLayer.Auth;
using EntityLayer.DTOs;
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
		IEmailSendMethod sendEmailMethod
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


	}
}
