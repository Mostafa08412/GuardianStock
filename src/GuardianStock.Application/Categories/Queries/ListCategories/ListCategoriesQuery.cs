using GuardianStock.Application.Categories.Queries;
using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Categories.Queries.ListCategories;

public record ListCategoriesQuery(
    string? SearchTerm,
    string? SortBy,
    int? MinProductCount,
    int? MaxProductCount,
    bool SortDescending = false,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<PaginatedList<CategoryListItemDto>>>;
