using FluentValidation;

namespace IMS.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{


    public UpdateProductCommandValidator()
    {

        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .PrecisionScale(18, 2, false)
            .When(x => x.Price.HasValue);

        RuleFor(x => x.Supplier)
            .MaximumLength(100)
            .When(x => x.Supplier is not null);
    }
}
