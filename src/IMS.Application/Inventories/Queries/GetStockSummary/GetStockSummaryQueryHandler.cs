using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Inventories.Queries.GetStockSummary
{
    public class GetStockSummaryQueryHandler : IRequestHandler<GetStockSummaryQuery, Result<StockSummaryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetStockSummaryQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<StockSummaryDto>> Handle(GetStockSummaryQuery request, CancellationToken cancellationToken)
        {
            var lowCount = await _context.Inventories.Where(x =>
             ((decimal)x.Quantity / x.LowStockThreshold) > 0.3m &&
             ((decimal)x.Quantity / x.LowStockThreshold) < 1.0m
             ).CountAsync(cancellationToken);

            var criticalCount = await _context.Inventories.Where(x =>
               ((decimal)x.Quantity / x.LowStockThreshold) <= 0.3m
               ).CountAsync(cancellationToken);

            var normalCount = await _context.Inventories.Where(x =>
               ((decimal)x.Quantity / x.LowStockThreshold) >= 1.0m
               ).CountAsync(cancellationToken);

            var dto = new StockSummaryDto
            {
                totalCriticalStock = criticalCount,
                totalLowStock = lowCount,
                totalNormalStock = normalCount
            };

            return Result<StockSummaryDto>.Success(dto);
        }
    }
}
