using FluentValidation;

namespace IMS.Application.Auth.GoogleLogin
{
    public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
    {
        public GoogleLoginCommandValidator()
        {
            RuleFor(x => x.IdToken).NotEmpty().NotNull();
        }
    }
}
