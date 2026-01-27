using FluentValidation;

namespace IMS.Application.Auth.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(X => X.refreshToken).NotEmpty().NotNull();
        }
    }

}
