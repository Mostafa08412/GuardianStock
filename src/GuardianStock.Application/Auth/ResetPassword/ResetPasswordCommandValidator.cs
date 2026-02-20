using FluentValidation;
using GuardianStock.Application.Common.Errors;

namespace GuardianStock.Application.Auth.ResetPassword
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
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
            RuleFor(x => x.ResetPasswordToken)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.ResetPasswordTokenIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.ResetPasswordTokenIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.ResetPasswordTokenIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.ResetPasswordTokenIsRequired.Code);
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code);
        }
    }
}
