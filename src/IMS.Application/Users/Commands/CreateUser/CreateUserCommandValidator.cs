using FluentValidation;
using IMS.Domain.Enums;

namespace IMS.Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters");


        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => role.Equals(Roles.Admin, StringComparison.OrdinalIgnoreCase) || role.Equals(Roles.Manager, StringComparison.OrdinalIgnoreCase) || role.Equals(Roles.Staff, StringComparison.OrdinalIgnoreCase)) // ignore case 

            .WithMessage("Role must be Admin, Manager, or Staff");



    }
}
