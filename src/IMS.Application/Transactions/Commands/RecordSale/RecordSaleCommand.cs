using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Transactions.Commands.RecordSale
{
    public record RecordSaleCommand : IRequest<Result>, ICommandRequest
    {
        public Guid productId { get; init; }
        public int quantity { get; init; }
    }
}
