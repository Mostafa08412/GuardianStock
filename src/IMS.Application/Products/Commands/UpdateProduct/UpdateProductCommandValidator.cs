using FluentValidation;
using IMS.Domain.Core.Errors;

namespace IMS.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{


    public UpdateProductCommandValidator()
    {

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage(Errors.ProductErrors.ProductIdIsRequired.Description)
            .WithErrorCode(Errors.ProductErrors.ProductIdIsRequired.Code);

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .WithMessage(Errors.ProductErrors.NameTooLong.Description)
            .WithErrorCode(Errors.ProductErrors.NameTooLong.Code)
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage(Errors.ProductErrors.DescriptionTooLong.Description)
            .WithErrorCode(Errors.ProductErrors.DescriptionTooLong.Code)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(Errors.ProductErrors.InvalidPrice.Description)
            .WithErrorCode(Errors.ProductErrors.InvalidPrice.Code)
            .PrecisionScale(18, 2, false)
            .WithMessage(Errors.ProductErrors.InvalidPrice.Description)
            .WithErrorCode(Errors.ProductErrors.InvalidPrice.Code)
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Supplier)
            .MaximumLength(100)
            .WithMessage(Errors.ProductErrors.SupplierTooLong.Description)
            .WithErrorCode(Errors.ProductErrors.SupplierTooLong.Code)
            .When(x => x.Supplier is not null);
    }
}
