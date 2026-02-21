using FluentValidation;
using GuardianStock.Application.Common.Errors;

namespace GuardianStock.Application.Auth.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {

            RuleFor(X => X.CurrentPassword)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.CurrentPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.CurrentPasswordIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.CurrentPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.CurrentPasswordIsRequired.Code);
            RuleFor(X => X.NewPassword)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code);
            RuleFor(X => X.ConfirmNewPassword)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.NewPasswordIsRequired.Code)
                .Equal(X => X.NewPassword)
                .WithMessage(ApplicationErrors.IdentityErrors.ConfirmPasswordMismatch.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.ConfirmPasswordMismatch.Code);
        }
    }
}
