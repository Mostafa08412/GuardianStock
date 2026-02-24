using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.LowStockAlerts.Queries.ListLowStockAlerts;

public record ListLowStockAlertsQuery(
    string? SearchTerm,
    string? Severity,
    bool? IsNotificationSent,
    bool? IsDismissed,
    DateTime? FromDate,
    DateTime? ToDate,
    string? SortBy,
    bool SortDescending = true,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<LowStockAlertListItemDto>>>;
