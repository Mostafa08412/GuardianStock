using FluentValidation;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.Transactions.Queries.GetTransaction
{
    public class GetTransactionQueryValidator : AbstractValidator<GetTransactionQuery>
    {
        public GetTransactionQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage(Errors.TransactionErrors.IdIsRequired.Description)
                .WithErrorCode(Errors.TransactionErrors.IdIsRequired.Code);
        }
    }


}
