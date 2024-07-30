using EntityLayer.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.Auth.Abstract;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Route("api/AuthServices")]
	[ApiController]
	public class AuthController(
		IAuthService authService
		) : ControllerBase
	{


		[HttpPost("Register")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Register([FromBody] UserDTO model)
		{

			var response = await authService.RegisterUserAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);

		}

		[HttpPost("Login")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Login([FromBody] LoginDTO model)
		{
			var response = await authService.LoginAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response);

		}

		[HttpPost("ForgotPassword")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
		{
			var response = await authService.ForgotPasswordAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}

		[HttpPost("ResetPassword")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
		{
			var response = await authService.ResetPasswordAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}
	}
}
