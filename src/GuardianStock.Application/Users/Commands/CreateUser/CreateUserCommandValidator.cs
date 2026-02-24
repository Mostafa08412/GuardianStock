using FluentValidation;
using GuardianStock.Domain.Enums;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(Errors.UserErrors.FirstNameIsRequired.Description)
            .WithErrorCode(Errors.UserErrors.FirstNameIsRequired.Code)
            .MaximumLength(100)
            .WithMessage(Errors.UserErrors.FirstNameTooLong.Description)
            .WithErrorCode(Errors.UserErrors.FirstNameTooLong.Code);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(Errors.UserErrors.LastNameIsRequired.Description)
            .WithErrorCode(Errors.UserErrors.LastNameIsRequired.Code)
            .MaximumLength(100)
            .WithMessage(Errors.UserErrors.LastNameTooLong.Description)
            .WithErrorCode(Errors.UserErrors.LastNameTooLong.Code);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(Errors.UserErrors.EmailIsRequired.Description)
            .WithErrorCode(Errors.UserErrors.EmailIsRequired.Code)
            .EmailAddress()
            .WithMessage(Errors.UserErrors.InvalidEmail.Description)
            .WithErrorCode(Errors.UserErrors.InvalidEmail.Code)
            .MaximumLength(256)
            .WithMessage(Errors.UserErrors.EmailTooLong.Description)
            .WithErrorCode(Errors.UserErrors.EmailTooLong.Code);


        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage(Errors.UserErrors.RoleIsRequired.Description)
            .WithErrorCode(Errors.UserErrors.RoleIsRequired.Code)
            .Must(role => role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase) || role.Equals(Roles.Manager, StringComparison.OrdinalIgnoreCase) || role.Equals(Roles.Staff, StringComparison.OrdinalIgnoreCase)) // ignore case 

            .WithMessage(Errors.UserErrors.InvalidRole.Description)
            .WithErrorCode(Errors.UserErrors.InvalidRole.Code);



    }
}
