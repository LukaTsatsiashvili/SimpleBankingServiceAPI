using EntityLayer.DTOs;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.FluentValidation;
using ServiceLayer.Services.Abstract;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Route("api/AuthServices")]
	[ApiController]
	public class UserController(
		IUserService userService
		) : ControllerBase
	{
		[HttpPost("Register")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Register([FromBody] UserDTO model)
		{

			var response = await userService.RegisterUserAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response);

		}

		[HttpPost("Login")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Login([FromBody] LoginDTO model)
		{

			var response = await userService.LoginAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response);


		}
	}
}
