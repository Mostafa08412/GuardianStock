using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Auth.GoogleLogin
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
