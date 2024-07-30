using EntityLayer.DTOs.Transaction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Services.API.User.Abstract;
using System.Security.Claims;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Route("api/TransactionServices")]
	[Authorize]
	[ApiController]
	public class TransactionController(ITransactionService service) : ControllerBase
	{
		[HttpPost("CreateTransaction")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateDTO model)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId is null || model is null) return BadRequest("Invalid Request!");

			var result = await service.CreateTransactionAsync(userId, model);
			if (!result.Flag) return BadRequest(result.Message);

			return Ok(result);
		}
	}
}
