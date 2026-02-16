using FluentValidation;
using IMS.Domain.Core.Errors;

namespace IMS.Application.Products.Commands.DeleteProduct;

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
