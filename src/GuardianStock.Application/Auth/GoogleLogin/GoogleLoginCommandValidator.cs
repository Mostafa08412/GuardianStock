using FluentValidation;
using GuardianStock.Application.Common.Errors;

namespace GuardianStock.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.GoogleIdTokenIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.GoogleIdTokenIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.GoogleIdTokenIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.GoogleIdTokenIsRequired.Code);
        }
    }
}
