using FluentValidation;
using IMS.Domain.Abstractions;

namespace IMS.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandValidator(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .PrecisionScale(18, 2, false);

        RuleFor(x => x.Supplier)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.InitialQuantity)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.LowStockThreshold)
            .GreaterThan(0);
    }
}
