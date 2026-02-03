using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Categories.Queries.ListCategories;

public class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesQuery, Result<PaginatedList<CategoryDto>>>
{
    private readonly IApplicationDbContext _context;

    public ListCategoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<CategoryDto>>> Handle(ListCategoriesQuery request, CancellationToken cancellationToken)
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
                "productcount" => request.SortDescending 
                    ? query.OrderByDescending(c => _context.Products.Count(p => p.CategoryId == c.Id)) 
                    : query.OrderBy(c => _context.Products.Count(p => p.CategoryId == c.Id)),
                _ => query.OrderBy(c => c.Name)
            };
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                _context.Products.Count(p => p.CategoryId == c.Id)))
            .ToListAsync(cancellationToken);

        return Result<PaginatedList<CategoryDto>>.Success(new PaginatedList<CategoryDto>(items, totalCount, request.Page, request.PageSize));
    }
}
