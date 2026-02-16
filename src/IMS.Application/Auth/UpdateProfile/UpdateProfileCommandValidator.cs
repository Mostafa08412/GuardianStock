using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Auth.UpdateProfile
{
    public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(X => X.FirstName)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Code)
                .MaximumLength(100)
                .WithMessage(ApplicationErrors.IdentityErrors.FirstNameTooLong.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.FirstNameTooLong.Code);
            RuleFor(X => X.LastName)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.LastNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.LastNameIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.LastNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.LastNameIsRequired.Code)
                .MaximumLength(100)
                .WithMessage(ApplicationErrors.IdentityErrors.LastNameTooLong.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.LastNameTooLong.Code);
        }
    }
}
