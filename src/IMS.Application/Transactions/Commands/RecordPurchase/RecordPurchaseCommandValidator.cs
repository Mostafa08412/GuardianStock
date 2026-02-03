using FluentValidation;

namespace IMS.Application.Transactions.Commands.RecordPurchase
{
    public class RecordPurchaseCommandValidator : AbstractValidator<RecordPurchaseCommand>
    {
        public RecordPurchaseCommandValidator()
        {
            RuleFor(X => X.productId)
                .NotEmpty()
                .NotNull()
                .NotEqual(Guid.Empty);

            RuleFor(X => X.quantity)
                .GreaterThan(0);


        }
    }
}
