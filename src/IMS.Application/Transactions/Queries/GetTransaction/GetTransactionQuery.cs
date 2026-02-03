using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Transactions.Queries.GetTransaction
{
    public record GetTransactionQuery(Guid Id) : IRequest<Result<TransactionDetailsDto>>;


}
