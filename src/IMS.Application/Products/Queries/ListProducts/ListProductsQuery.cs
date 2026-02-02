using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Queries.ListProducts;

public record ListProductsQuery(
    string? SearchTerm,
    Guid? CategoryId,
    string? StockLevel,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? SortBy,
    bool SortDescending = false,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<ProductListItemDto>>>;
