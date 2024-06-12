using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.API.User.Abstract;
using System.Security.Claims;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Route("api/AccountServices")]
	[Authorize]
	[ApiController]
	public class AccountController(IAccountService service) : ControllerBase
	{
		[HttpPost("CreateAccount")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> CreateAccount()
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null) return BadRequest("Invalid request!");

			var response = await service.CreateAccountAsync(userId);
			if (response.Flag == false) return BadRequest("Failed to create account!");

			return Ok(response.Message);
		}
	}
}
