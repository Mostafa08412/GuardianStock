using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<CategoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDto>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                _context.Products.Count(p => p.CategoryId == c.Id)))
            .FirstOrDefaultAsync(cancellationToken);

        if (category is null)
        {
            return Result<CategoryDto>.Failure(Errors.CategoryErrors.NotFound);
        }

        return Result<CategoryDto>.Success(category);
    }
}
