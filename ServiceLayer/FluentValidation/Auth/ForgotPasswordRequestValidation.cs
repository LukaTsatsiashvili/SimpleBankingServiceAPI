using EntityLayer.DTOs.Auth;
using FluentValidation;
using ServiceLayer.Messages;

namespace ServiceLayer.FluentValidation.Auth
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
