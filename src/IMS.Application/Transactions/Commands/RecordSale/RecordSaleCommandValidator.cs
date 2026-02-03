using FluentValidation;

namespace IMS.Application.Transactions.Commands.RecordSale
{
    public class RecordSaleCommandValidator : AbstractValidator<RecordSaleCommand>
    {
        public RecordSaleCommandValidator()
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
