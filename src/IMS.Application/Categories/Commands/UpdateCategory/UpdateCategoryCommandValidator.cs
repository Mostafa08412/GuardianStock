using FluentValidation;
using IMS.Domain.Core.Errors;

namespace IMS.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(Errors.CategoryErrors.IdIsRequired.Description)
            .WithErrorCode(Errors.CategoryErrors.IdIsRequired.Code);

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage(Errors.CategoryErrors.NameTooLong.Description)
            .WithErrorCode(Errors.CategoryErrors.NameTooLong.Code)
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(Errors.CategoryErrors.DescriptionTooLong.Description)
            .WithErrorCode(Errors.CategoryErrors.DescriptionTooLong.Code)
            .When(x => x.Description is not null);
    }
}
