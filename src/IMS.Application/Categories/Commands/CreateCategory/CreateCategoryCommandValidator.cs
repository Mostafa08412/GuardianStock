using FluentValidation;
using IMS.Domain.Core.Errors;

namespace IMS.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Errors.CategoryErrors.NameIsRequired.Description)
            .WithErrorCode(Errors.CategoryErrors.NameIsRequired.Code)
            .MaximumLength(100)
            .WithMessage(Errors.CategoryErrors.NameTooLong.Description)
            .WithErrorCode(Errors.CategoryErrors.NameTooLong.Code);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(Errors.CategoryErrors.DescriptionIsRequired.Description)
            .WithErrorCode(Errors.CategoryErrors.DescriptionIsRequired.Code)
            .MaximumLength(500)
            .WithMessage(Errors.CategoryErrors.DescriptionTooLong.Description)
            .WithErrorCode(Errors.CategoryErrors.DescriptionTooLong.Code);
    }
}
