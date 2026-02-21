using FluentValidation;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.Transactions.Commands.RecordPurchase
{
    public class RecordPurchaseCommandValidator : AbstractValidator<RecordPurchaseCommand>
    {
        public RecordPurchaseCommandValidator()
        {
            RuleFor(X => X.productId)
                .NotEmpty()
                .WithMessage(Errors.TransactionErrors.InvalidProductId.Description)
                .WithErrorCode(Errors.TransactionErrors.InvalidProductId.Code)
                .NotNull()
                .WithMessage(Errors.TransactionErrors.InvalidProductId.Description)
                .WithErrorCode(Errors.TransactionErrors.InvalidProductId.Code)
                .NotEqual(Guid.Empty)
                .WithMessage(Errors.TransactionErrors.InvalidProductId.Description)
                .WithErrorCode(Errors.TransactionErrors.InvalidProductId.Code);

            RuleFor(X => X.quantity)
                .GreaterThan(0)
                .WithMessage(Errors.TransactionErrors.InvalidQuantity.Description)
                .WithErrorCode(Errors.TransactionErrors.InvalidQuantity.Code);


        }
    }
}
