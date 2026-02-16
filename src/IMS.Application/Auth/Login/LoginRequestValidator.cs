using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Auth.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {

            RuleFor(X => X.Email)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.EmailIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.EmailIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code);

            RuleFor(X => X.Password)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.PasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.PasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code);
        }
    }

}


