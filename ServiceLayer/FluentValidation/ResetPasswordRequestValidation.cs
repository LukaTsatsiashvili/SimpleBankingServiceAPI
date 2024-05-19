using EntityLayer.DTOs.Auth;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation
{
    public class ResetPasswordRequestValidation : AbstractValidator<ResetPasswordDTO>
	{
        public ResetPasswordRequestValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("UserId"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("UserId"));

            RuleFor(x => x.Token)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Token"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Token"));

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("New Password"))
                .NotNull().WithMessage(ValidationMessages.NullEmptyMessage("New Password"))
                .MinimumLength(7).WithMessage(ValidationMessages.GreaterThanMessage("New Password", 7));
		}
    }
}
