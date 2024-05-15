using EntityLayer.DTOs;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation
{
	public class ForgotPasswordRequestValidation : AbstractValidator<ForgotPasswordDTO>
	{
        public ForgotPasswordRequestValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
                .NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
				.EmailAddress().WithMessage(ValidationMessages.EmailMessage("Email"));
        }
    }
}
