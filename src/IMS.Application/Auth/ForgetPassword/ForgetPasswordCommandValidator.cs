using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Auth.ForgetPassword
{
    public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public ForgetPasswordCommandValidator()
        {
            RuleFor(x => x.EmailAddress)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.EmailIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.EmailIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .EmailAddress()
                .WithMessage(ApplicationErrors.IdentityErrors.InvalidEmail.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.InvalidEmail.Code);
        }
    }
}
