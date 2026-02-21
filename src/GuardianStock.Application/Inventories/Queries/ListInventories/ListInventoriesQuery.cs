using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Inventories.Queries.ListInventories;

public record ListInventoriesQuery(
    string? SearchTerm,
    string? StockStatus,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    bool SortDescending = false,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<InventoryListItemDto>>>;
