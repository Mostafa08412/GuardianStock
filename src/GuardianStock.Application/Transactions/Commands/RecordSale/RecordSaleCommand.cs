using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Transactions.Commands.RecordSale
{
    public record RecordSaleCommand : IRequest<Result>
    {
        public Guid productId { get; init; }
        public int quantity { get; init; }
    }
}
