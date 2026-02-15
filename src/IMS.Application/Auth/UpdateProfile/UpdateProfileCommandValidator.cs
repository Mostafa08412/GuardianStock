using FluentValidation;

namespace IMS.Application.Auth.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(X => X.FirstName).NotEmpty().NotNull().MaximumLength(100);
            RuleFor(X => X.LastName).NotEmpty().NotNull().MaximumLength(100);
        }
    }
}
