using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Categories.Queries.ListCategories;

public record ListCategoriesQuery(
    string? SearchTerm,
    string? SortBy,
    int? MinProductCount,
    int? MaxProductCount,
    bool SortDescending = false,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<CategoryListItemDto>>>;
