using FluentValidation;

namespace IMS.Application.Auth.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {

            RuleFor(X => X.Email).NotEmpty().NotNull();

            RuleFor(X => X.Password).NotEmpty().NotNull();
        }
    }

}

