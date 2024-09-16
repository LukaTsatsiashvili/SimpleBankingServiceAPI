using Microsoft.AspNetCore.Http;

namespace EntityLayer.DTOs;

public record MonthlyReportDTO(
	string FileName,
	IFormFile File);
