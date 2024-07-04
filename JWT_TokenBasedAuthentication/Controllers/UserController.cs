using EntityLayer.DTOs.Auth;
using EntityLayer.DTOs.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Helpers;
using ServiceLayer.Services.API.User.Abstract;
using System.Security.Claims;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Authorize]
	[Route("api/UserServices")]
	[ApiController]
	public class UserController(
		IUserService userService,
		IFileValidator fileValidator
		) : ControllerBase
	{

		[HttpPost("UploadImage")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> UploadProfilePicture([FromForm] ImageUploadDTO model)
		{
			var validation = fileValidator.ValidateFile(model);
			if (!validation.Flag) return BadRequest(validation.Message);

			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null) return BadRequest("Something went wrong! Please try again later.");

			var response = await userService.UploadProfilePictureAsync(userId, model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}

		[HttpDelete("RemoveProfilePicture")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> RemoveProfilePicture()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null) return BadRequest("Something went wrong! Please try again later.");

			var response = await userService.RemoveProfilePictureAsync(userId);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}

		[HttpPost("ChangePassword")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null) return BadRequest("Something went wrong! Please try again later.");

			var response = await userService.ChangePasswordAsync(userId, model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}

		[HttpGet("GetUserInformation")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetUserInformation()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null) return BadRequest("Unauthorized");

			var result = await userService.GetUserInformationAsync(userId);
			if (result.Flag == false) return BadRequest(result.Message);

			return Ok(result.Data);
		}

		[HttpDelete("DeleteAccount")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> DeleteAccount()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null) return BadRequest("Something went wrong! Please try again later.");

			var response = await userService.DeleteUserAsync(userId);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}
	}
}
