using EntityLayer.DTOs.User;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation.API.User;

public class UserCreateRequestValidation : AbstractValidator<CreateUserDTO>
{
	public UserCreateRequestValidation()
	{

		RuleFor(x => x.Password)
			.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Password"))
			.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Password"))
			.MinimumLength(7).WithMessage(ValidationMessages.GreaterThanMessage("Password", 7));

		RuleFor(x => x.PhoneNumber)
				.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("PhoneNumber"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("PhoneNumber"))
				.Matches(@"^\d+$").WithMessage(ValidationMessages.PhoneNumberFormatMessage("PhoneNumber"))
				.Length(9).WithMessage(ValidationMessages.MustBeEqualOfConcreteNumber("PhoneNumber", 9));

		RuleFor(x => x.Email)
				.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
				.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
				.EmailAddress().WithMessage(ValidationMessages.EmailMessage("Email"));

		RuleFor(x => x.Role)
			.NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
			.NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
			.Must(BeAValidRole).WithMessage(ValidationMessages.MustBeCorrectRole("Role"));
	}

	private bool BeAValidRole(string role)
	{
		var allowedRoles = new[] { "user" };
		return allowedRoles.Contains(role.ToLower());
	}
}
