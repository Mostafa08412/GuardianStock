using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Inventories.Queries.GetStockSummary
{
    public record GetStockSummaryQuery : IRequest<Result<StockSummaryDto>>;

}
