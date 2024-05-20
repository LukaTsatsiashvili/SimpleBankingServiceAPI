using EntityLayer.DTOs.Image;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Helpers
{
	public interface IFileValidator
	{
		GeneralResponse ValidateFile(ImageUploadDTO file);
	}
	public class FileValidator : IFileValidator
	{
		public GeneralResponse ValidateFile(ImageUploadDTO model)
		{
			var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

			if (!allowedExtensions.Contains(Path.GetExtension(model.File.FileName)))
			{
				return new GeneralResponse(false, "Only '.jpg', '.jpeg' or '.png' file extensions are supported!");
			}

			if (model.File.Length > 10485760)
			{
				return new GeneralResponse(false, "Only files with size 10MB or less are allowed!");
			}

			return new GeneralResponse(true, null!);
		}
	}
}
