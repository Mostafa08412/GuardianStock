using FluentValidation;
using IMS.Application.Common.Errors;

namespace IMS.Application.Auth.Register
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage(ApplicationErrors.IdentityErrors.InvalidEmail.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.InvalidEmail.Code)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.EmailIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.EmailIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.EmailIsRequired.Code);
            RuleFor(x => x.FirstName)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Code)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.FirstNameIsRequired.Code);
            RuleFor(x => x.LastName)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.LastNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.LastNameIsRequired.Code)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.LastNameIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.LastNameIsRequired.Code);
            RuleFor(x => x.Password)
                .NotNull()
                .WithMessage(ApplicationErrors.IdentityErrors.PasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code)
                .NotEmpty()
                .WithMessage(ApplicationErrors.IdentityErrors.PasswordIsRequired.Description)
                .WithErrorCode(ApplicationErrors.IdentityErrors.PasswordIsRequired.Code);
        }
    }
}
