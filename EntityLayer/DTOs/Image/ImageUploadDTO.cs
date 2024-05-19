using Microsoft.AspNetCore.Http;

namespace EntityLayer.DTOs.Image
{
	public class ImageUploadDTO
	{
		public IFormFile File { get; set; } = null!;
		public string FileName { get; set; } = null!;
	}
}
