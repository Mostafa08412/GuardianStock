using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace IMS.Application.LowStockAlerts.Queries.GetLowStockAlerts;

public class GetLowStockAlertsQueryHandler : IRequestHandler<GetLowStockAlertsQuery, Result<PaginatedList<LowStockAlertDto>>>
{
    private readonly IApplicationDbContext _context;

    public GetLowStockAlertsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<LowStockAlertDto>>> Handle(GetLowStockAlertsQuery request, CancellationToken cancellationToken)
    {

        var query = from inventory in _context.Inventories
                    .AsNoTracking()
                    .Where(X => (X.LowStockAlert != null))
                    join product in _context.Products.AsNoTracking()
                    on inventory.ProductId equals product.Id
                    select new
                    {
                        Id = inventory.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductSku = product.Sku,
                        CurrentStock = inventory.Quantity,
                        LowStockThreshold = inventory.LowStockThreshold,
                        AlertTriggeredAt = inventory.LowStockAlert!.TriggeredAtUTC,
                        DismissedAt = inventory.LowStockAlert!.DismissedAt,
                        IsNotificationSent = inventory.LowStockAlert!.NotificationSentAt != default
                    };

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x =>
                x.ProductName.Contains(request.SearchTerm) ||
                x.ProductSku.Contains(request.SearchTerm));
        }

        if (request.IsDismissed.HasValue && request.IsDismissed.Value)
        {
            query = query.Where(x => x.DismissedAt != default);
        }


        if (request.IsDismissed.HasValue && !request.IsDismissed.Value)
        {
            query = query.Where(x => x.DismissedAt == default);
        }

        if (request.IsNotificationSent.HasValue)
        {
            query = query.Where(x => x.IsNotificationSent == request.IsNotificationSent.Value);
        }

        if (request.FromDate.HasValue)
        {
            query = query.Where(x => x.AlertTriggeredAt >= request.FromDate.Value);
        }

        if (request.ToDate.HasValue)
        {
            query = query.Where(x => x.AlertTriggeredAt <= request.ToDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Severity))
        {
            if (request.Severity == "critical")
            {
                query = query.Where(x => ((decimal)x.CurrentStock / x.LowStockThreshold) <= 0.3m);
            }
            else if (request.Severity == "low")
            {
                query = query.Where(x =>
                                       ((decimal)x.CurrentStock / x.LowStockThreshold) > 0.3m &&
                                       ((decimal)x.CurrentStock / x.LowStockThreshold) <= 1.0m);
            }

        }

        query = request.SortBy?.ToLower() switch
        {
            "productname" => request.SortDescending ? query.OrderByDescending(x => x.ProductName) : query.OrderBy(x => x.ProductName),
            "date" => request.SortDescending ? query.OrderByDescending(x => x.AlertTriggeredAt) : query.OrderBy(x => x.AlertTriggeredAt),
            "severity" => request.SortDescending
                ? query.OrderByDescending(x => (decimal)x.CurrentStock / x.LowStockThreshold)
                : query.OrderBy(x => (decimal)x.CurrentStock / x.LowStockThreshold),
            _ => request.SortDescending ? query.OrderByDescending(x => x.AlertTriggeredAt) : query.OrderBy(x => x.AlertTriggeredAt)
        };


        var projectedQuery = query.Select(x => new LowStockAlertDto(
            x.Id,
            x.ProductId,
            x.ProductName,
            x.ProductSku,
            x.CurrentStock,
            x.LowStockThreshold,
            ((decimal)x.CurrentStock / x.LowStockThreshold) <= 0.3m ? "critical" :
                      ((decimal)x.CurrentStock / x.LowStockThreshold) < 1.0m ? "low" : "normal",
            x.IsNotificationSent,
            x.DismissedAt != null,
            x.AlertTriggeredAt.ToLocalTime()));

        var alerts = await projectedQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);

        return Result<PaginatedList<LowStockAlertDto>>.Success(new PaginatedList<LowStockAlertDto>(alerts, totalCount, request.Page, request.PageSize));
    }


}
