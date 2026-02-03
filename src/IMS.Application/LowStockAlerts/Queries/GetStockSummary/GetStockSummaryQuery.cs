using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Queries.GetStockSummary
{
    public record GetStockSummaryQuery : IRequest<Result<StockSummaryDto>>;

}
