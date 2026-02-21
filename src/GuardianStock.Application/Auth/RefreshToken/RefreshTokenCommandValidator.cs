using FluentValidation;
using GuardianStock.Application.Common.Errors;

namespace GuardianStock.Application.Auth.RefreshToken
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
