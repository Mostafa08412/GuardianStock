using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Categories.Queries.GetCategory;

public class GetCategoryQueryHandler : IRequestHandler<GetCategoryQuery, Result<CategoryDetails>>
{
    private readonly IApplicationDbContext _context;

    public GetCategoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CategoryDetails>> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        var totalValueQuery = from product in _context.Products.AsNoTracking().Where(X => X.CategoryId == request.Id)
                              join inventory in _context.Inventories.AsNoTracking()
                              on product.Id equals inventory.ProductId
                              select new
                              {
                                  UnitPrice = product.Price,
                                  Stock = inventory.Quantity
                              };


        var categoryDetails = await (from category in _context.Categories.AsNoTracking().Where(X => X.Id == request.Id)
                                     join product in _context.Products.AsNoTracking()
                                     on category.Id equals product.CategoryId
                                     select new CategoryDetails
                                     (
                                         category.Id,
                                         category.Name,
                                         category.Description,
                                        _context.Products.AsNoTracking().Where(X => X.CategoryId == request.Id).Count(),
                                         totalValueQuery.Sum(X => X.Stock * X.UnitPrice),
                                         totalValueQuery.Average(X => X.UnitPrice),
                                         totalValueQuery.Sum(X => X.Stock)
                                     )).FirstOrDefaultAsync(cancellationToken);



        if (categoryDetails is null)
        {
            return Result<CategoryDetails>.Failure(Errors.CategoryErrors.NotFound);
        }

        return Result<CategoryDetails>.Success(categoryDetails);
    }
}
