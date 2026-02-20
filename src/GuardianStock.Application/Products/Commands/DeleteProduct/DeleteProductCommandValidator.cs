using FluentValidation;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage(Errors.ProductErrors.ProductIdIsRequired.Description)
            .WithErrorCode(Errors.ProductErrors.ProductIdIsRequired.Code);
    }
}
