using GuardianStock.Application.Categories.Queries;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Application.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, Result<PaginatedList<CategoryListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public ListCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<CategoryListItemDto>>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm) || c.Description.Contains(request.SearchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "products" => request.SortDescending
                    ? query.OrderByDescending(c => _context.Products.Count(p => p.CategoryId == c.Id))
                    : query.OrderBy(c => _context.Products.Count(p => p.CategoryId == c.Id)),
                _ => query.OrderBy(c => c.Name)
            };
        }


        if (request.MinProductCount.HasValue)
            query = query.Where(c => _context.Products.Count(p => p.CategoryId == c.Id) >= request.MinProductCount.Value);
        if (request.MaxProductCount.HasValue)
            query = query.Where(c => _context.Products.Count(p => p.CategoryId == c.Id) <= request.MaxProductCount.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryListItemDto(
                c.Id,
                c.Name,
                c.Description,
                _context.Products.Count(p => p.CategoryId == c.Id)))
            .ToListAsync(cancellationToken);

        return Result<PaginatedList<CategoryListItemDto>>.Success(new PaginatedList<CategoryListItemDto>(items, totalCount, request.Page, request.PageSize));
    }
}
