using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Inventories.Queries.ListInventories;

public record ListInventoriesQuery(
    string? SearchTerm,
    string? StockStatus,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    bool SortDescending = false,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<InventoryListItemDto>>>;
