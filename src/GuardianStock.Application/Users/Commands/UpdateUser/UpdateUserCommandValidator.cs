using FluentValidation;
using GuardianStock.Domain.Enums;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(Errors.UserErrors.IdIsRequired.Description)
            .WithErrorCode(Errors.UserErrors.IdIsRequired.Code);

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


        RuleFor(x => x.Role)
            .NotEmpty()
            .WithMessage(Errors.UserErrors.RoleIsRequired.Description)
            .WithErrorCode(Errors.UserErrors.RoleIsRequired.Code)
            .Must(role => role == Roles.Admin || role == Roles.Manager || role == Roles.Staff)
            .WithMessage(Errors.UserErrors.InvalidRole.Description)
            .WithErrorCode(Errors.UserErrors.InvalidRole.Code);
    }
}
