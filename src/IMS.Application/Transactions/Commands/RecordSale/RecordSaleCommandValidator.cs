using FluentValidation;

namespace IMS.Application.Transactions.Queries.RecordSale
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
