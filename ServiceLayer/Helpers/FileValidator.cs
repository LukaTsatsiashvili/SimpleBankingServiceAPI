using EntityLayer.DTOs;
using EntityLayer.DTOs.Image;
using static ServiceLayer.Responses.ServiceResponses;

namespace ServiceLayer.Helpers
{
	public interface IFileValidator
	{
		GeneralResponse ValidateFile(ImageUploadDTO file);
		GeneralResponse ValidateExcelFile(MonthlyReportDTO model);
	}
	public class FileValidator : IFileValidator
	{
		public GeneralResponse ValidateExcelFile(MonthlyReportDTO model)
		{
			if (string.IsNullOrEmpty(model.FileName)) return new GeneralResponse(false, "File Name must be provided!");

			var allowedExtensions = new string[] { ".xlsx" };

			if (model.File is null || model.File.Length == 0)
			{
				return new GeneralResponse(false, "File must be provided!");
			}

			if (!allowedExtensions.Contains(Path.GetExtension(model.File.FileName)))
			{
				return new GeneralResponse(false, "Only '.xlsx.' files are supported! ");
			}

			return new GeneralResponse(true, null!);
		}

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
