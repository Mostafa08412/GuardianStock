using FluentValidation;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(Errors.ProductErrors.NameIsRequired.Description)
            .WithErrorCode(Errors.ProductErrors.NameIsRequired.Code)
            .MaximumLength(100)
            .WithMessage(Errors.ProductErrors.NameTooLong.Description)
            .WithErrorCode(Errors.ProductErrors.NameTooLong.Code);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(Errors.ProductErrors.DescriptionIsRequired.Description)
            .WithErrorCode(Errors.ProductErrors.DescriptionIsRequired.Code)
            .MaximumLength(500)
            .WithMessage(Errors.ProductErrors.DescriptionTooLong.Description)
            .WithErrorCode(Errors.ProductErrors.DescriptionTooLong.Code);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage(Errors.ProductErrors.InvalidPrice.Description)
            .WithErrorCode(Errors.ProductErrors.InvalidPrice.Code)
            .PrecisionScale(18, 2, false)
            .WithMessage(Errors.ProductErrors.InvalidPrice.Description)
            .WithErrorCode(Errors.ProductErrors.InvalidPrice.Code);

        RuleFor(x => x.Supplier)
            .NotEmpty()
            .WithMessage(Errors.ProductErrors.SupplierIsRequired.Description)
            .WithErrorCode(Errors.ProductErrors.SupplierIsRequired.Code)
            .MaximumLength(100)
            .WithMessage(Errors.ProductErrors.SupplierTooLong.Description)
            .WithErrorCode(Errors.ProductErrors.SupplierTooLong.Code);

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage(Errors.ProductErrors.CategoryIsRequired.Description)
            .WithErrorCode(Errors.ProductErrors.CategoryIsRequired.Code);

        RuleFor(x => x.InitialQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage(Errors.ProductErrors.InvalidInitialQuantity.Description)
            .WithErrorCode(Errors.ProductErrors.InvalidInitialQuantity.Code);

        RuleFor(x => x.LowStockThreshold)
            .GreaterThan(0)
            .WithMessage(Errors.ProductErrors.InvalidLowStockThreshold.Description)
            .WithErrorCode(Errors.ProductErrors.InvalidLowStockThreshold.Code);
    }
}
