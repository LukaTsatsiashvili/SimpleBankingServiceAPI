using EntityLayer.DTOs;
using EntityLayer.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Helpers;
using ServiceLayer.Services.API.User.Abstract;

namespace JWT_TokenBasedAuthentication.Controllers
{
	[Route("api/AdminServices")]
	[Authorize(Roles = "Admin")]
	[ApiController]
	public class AdminController(
		IAdminService service,
		IFileValidator validator) : ControllerBase
	{
		[HttpGet("LoadAllUsers")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> LoadAllUsers()
		{
			var result = await service.GetAllUsersAsync();

			if (!result.Flag) return BadRequest(result.Message);

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

			if (!result.Flag) return BadRequest(result.Message);

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

			if (!result.Flag) return BadRequest(result.Message);

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
			if (!result.Flag) return BadRequest(result.Message);

			return Ok(result);
		}

		[HttpPost("CreateUser")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO model)
		{
			if (model == null) return BadRequest("Model is empty!");

			var result = await service.CreateUserAsync(model);
			if (!result.Flag) return BadRequest(result.Message);

			return Ok(result.Message);
		}


		[HttpGet("GenerateUserExcelFile")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GenerateUserExcelFile([FromQuery] Guid? id)
		{

			var result = await service.GenerateUserExcelFileAsync(id);

			if (!result.Flag) return BadRequest(result.Message);

			return File(result.File, result.ContentType, result.FileName);
		}

		[HttpGet("GenerateAuditLogExcelFile")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GenerateAuditLogExcelFile([FromQuery] string? userEmail)
		{
			var result = await service.GenerateAuditLogsExcelFileAsync(userEmail);

			if (!result.Flag) return BadRequest(result.Message);

			return File(result.File, result.ContentType, result.FileName);
		}

		[HttpPost("UploadMonthlyReportFile")]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> UploadMonthlyReportFile([FromForm] MonthlyReportDTO model)
		{
			var validation = validator.ValidateExcelFile(model);
			if (!validation.Flag) return BadRequest(validation.Message);

			var response = await service.ImportMonthlyReportFileToDBAsync(model);
			if (!response.Flag) return BadRequest(response.Message);

			return Ok(response.Message);
		}
	}
}
