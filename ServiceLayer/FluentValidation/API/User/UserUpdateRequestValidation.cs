using EntityLayer.DTOs.User;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation.API.User
{
	public class UserUpdateRequestValidation : AbstractValidator<UpdateUsersInformationDTO>
	{
        public UserUpdateRequestValidation()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
                .NotNull().WithMessage(ValidationMessages.NullEmptyMessage("Email"))
                .EmailAddress().WithMessage(ValidationMessages.EmailMessage("Email"));

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("UserName"))
                .NotNull().WithMessage(ValidationMessages.NullEmptyMessage("UserName"))
                .Matches(@"^\S*$").WithMessage(ValidationMessages.NoWhiteSpaceMessage("UserName"));

            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage(ValidationMessages.NullEmptyMessage("PhoneNumber"))
                .NotNull().WithMessage(ValidationMessages.NullEmptyMessage("PhoneNumber"))
                .Matches(@"^\d+$").WithMessage(ValidationMessages.PhoneNumberFormatMessage("PhoneNumber"))
                .Length(9).WithMessage(ValidationMessages.MustBeEqualOfConcreteNumber("PhoneNumber", 9));

		}
    }
}
