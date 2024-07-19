using EntityLayer.DTOs.User;
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

		[HttpGet("GetSingleUser/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> LoadSingleUser(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Provide valid ID!");

			var result = await service.GetSingleUserAsync(id);

			if (result.Flag == false) return BadRequest(result.Message);

			return Ok(result);
		}

		[HttpGet("GetUserTransactionHistory/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GetUserTransaction(Guid id)
		{
			if (id == Guid.Empty) return BadRequest("Provide proper ID!");

			var result = await service.GetUserTransactionsAsync(id);

			if (result.Flag == false) return BadRequest(result.Message);

			return Ok(result);
		}

		[HttpPut("UpdateUserInformation/{id:guid}")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> UpdateUserInformation(Guid id, [FromBody] UpdateUsersInformationDTO model)
		{
			if (id == Guid.Empty || model is null) return BadRequest("Provide proper information!");
			
			var result = await service.UpdateUserInformationAsync(id, model);
			if (result.Flag == false) return BadRequest(result.Message);

			return Ok(result);
		}
	}
}
