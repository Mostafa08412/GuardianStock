using FluentValidation;

namespace IMS.Application.Auth.ChangePassword
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {

            RuleFor(X => X.CurrentPassword).NotEmpty().NotNull();
            RuleFor(X => X.NewPassword).NotEmpty().NotNull();
            RuleFor(X => X.ConfirmNewPassword)
                .NotEmpty()
                .NotNull()
                .Equal(X => X.NewPassword).WithMessage("New password and confirm password do not match.");
        }
    }
}
