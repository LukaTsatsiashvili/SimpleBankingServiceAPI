using EntityLayer.DTOs.Image;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation.API.User
{
	public class ImageUploadRequestValidation : AbstractValidator<ImageUploadDTO>
	{
        public ImageUploadRequestValidation()
        {
			RuleFor(x => x.File)
				.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("File"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("File"));

			RuleFor(x => x.FileName)
				.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("FileName"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("FileName"))
				.MaximumLength(50).WithMessage(ValidationMessages.LessThanMessage("FileName", 50));

		}
    }
}
