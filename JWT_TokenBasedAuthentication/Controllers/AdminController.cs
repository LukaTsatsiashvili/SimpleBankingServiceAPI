using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.API.User.Abstract;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Route("api/AdminServices")]
	[Authorize(Roles = "Admin")]
	[ApiController]
	public class AdminController(IAdminService service) : ControllerBase
	{
		[HttpGet("LoadAllUsers")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> LoadAllUsers()
		{
			var result = await service.GetAllUsersAsync();

			if (result.Flag == false) return BadRequest(result.Message);

			return Ok(result);
		}
	}
}
