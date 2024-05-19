using EntityLayer.DTOs.Auth;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation
{
    public class LoginRequestValidation : AbstractValidator<LoginDTO>
	{
		public LoginRequestValidation()
		{
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
				.EmailAddress().WithMessage(ValidationMessages.EmailMessage("Email"));

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Password"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Password"))
				.MinimumLength(7).WithMessage(ValidationMessages.GreaterThanMessage("Password", 7));
		}
	}
}
