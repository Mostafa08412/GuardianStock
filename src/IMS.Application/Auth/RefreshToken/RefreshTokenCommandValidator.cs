using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Auth.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(X => X.refreshToken)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.RefreshTokenIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.RefreshTokenIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.RefreshTokenIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.RefreshTokenIsRequired.Code);
        }
    }

}
