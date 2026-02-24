using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Transactions.Queries.GetTransaction
{
    public record GetTransactionQuery(Guid Id) : IRequest<Result<TransactionDetailsDto>>;


}
