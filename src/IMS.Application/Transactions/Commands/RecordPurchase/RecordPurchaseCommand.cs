using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Transactions.Commands.RecordPurchase
{
    public record RecordPurchaseCommand : IRequest<Result>
    {
        public Guid productId { get; init; }
        public int quantity { get; init; }
    }
}
