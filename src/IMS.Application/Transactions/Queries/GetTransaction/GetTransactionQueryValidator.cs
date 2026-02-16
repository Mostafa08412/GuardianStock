using FluentValidation;
using IMS.Domain.Core.Errors;

namespace IMS.Application.Transactions.Queries.GetTransaction
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
