using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Queries.GetLowStockAlerts;

public record GetLowStockAlertsQuery(
    string? SearchTerm,
    string? Severity,
    bool? IsNotificationSent,
    bool? IsDismissed,
    DateTime? FromDate,
    DateTime? ToDate,
    string? SortBy,
    bool SortDescending = true,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<LowStockAlertDto>>>;
