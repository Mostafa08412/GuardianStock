using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Application.Products.Queries.ListProducts;

public class ListProductsQueryHandler : IRequestHandler<ListProductsQuery, Result<PaginatedList<ProductListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public ListProductsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<ProductListItemDto>>> Handle(ListProductsQuery request, CancellationToken cancellationToken)
    {

        var query = _context.Products.AsNoTracking().AsQueryable().OrderBy(X => X.Id).AsQueryable();
        var InventoryQuery = _context.Inventories.AsNoTracking().AsNoTracking();
        var CategoryQuery = _context.Categories.AsNoTracking().AsNoTracking();




        if (!string.IsNullOrEmpty(request.SortBy))
        {

            if (request.SortBy.ToLower() == "sku")
                query = request.SortDescending ? query.OrderByDescending(X => X.Sku) : query.OrderBy(X => X.Sku);
            if (request.SortBy.ToLower() == "name")
                query = request.SortDescending ? query.OrderByDescending(X => X.Name) : query.OrderBy(X => X.Name);
            if (request.SortBy.ToLower() == "price")
                query = request.SortDescending ? query.OrderByDescending(X => X.Price) : query.OrderBy(X => X.Price);
        }


        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {

            query = query.Where(X =>
            EF.Functions.Like(X.Name, $"%{request.SearchTerm}%") ||
            EF.Functions.Like(X.Sku, $"%{request.SearchTerm}%") ||
            EF.Functions.Like(X.Description, $"%{request.SearchTerm}%") ||
            EF.Functions.Like(X.Supplier, $"%{request.SearchTerm}%"));
        }

        if (request.CategoryId.HasValue && request.CategoryId != Guid.Empty)
        {

            query = query.Where(X => X.CategoryId == request.CategoryId);
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(X => X.Price >= request.MinPrice);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(X => X.Price <= request.MaxPrice);
        }

        var ProductsWithCategories = query.Join(
            CategoryQuery,
            (P) => P.CategoryId,
            (C) => C.Id,
            (P, C) => new
            {
                Id = P.Id,
                Name = P.Name,
                Sku = P.Sku,
                Description = P.Description,
                Price = P.Price,
                Supplier = P.Supplier,
                CategoryName = C.Name,
                CategoryId = C.Id,
                ImageUrl = P.ImageUrl
            });



        var ProductsWithCategoriesWithInventories = ProductsWithCategories.Join(
            InventoryQuery,
            (p) => p.Id,
            (i) => i.ProductId,
            (p, i) => new
            {
                p,
                i.Quantity,
                i.LowStockThreshold,
            });



        if (!string.IsNullOrWhiteSpace(request.StockLevel))
        {
            var stockLevel = request.StockLevel.ToLower();


            if (stockLevel == "normal")
            {
                ProductsWithCategoriesWithInventories = ProductsWithCategoriesWithInventories.Where(X => X.Quantity > X.LowStockThreshold);
            }

            else if (stockLevel == "low")
            {
                ProductsWithCategoriesWithInventories = ProductsWithCategoriesWithInventories
                    .Where(X => X.LowStockThreshold > 0)
                    .Where(X => (double)X.Quantity / X.LowStockThreshold > 0.3 && (double)X.Quantity / X.LowStockThreshold < 1);

            }
            else if (stockLevel == "critical")
            {
                ProductsWithCategoriesWithInventories = ProductsWithCategoriesWithInventories
                    .Where(X => (double)X.Quantity / X.LowStockThreshold <= 0.3);
            }
        }


        if (!string.IsNullOrEmpty(request.SortBy))
        {
            if (request.SortBy.ToLower() == "stock")
                ProductsWithCategoriesWithInventories = request.SortDescending ? ProductsWithCategoriesWithInventories.OrderByDescending(X => X.Quantity) : ProductsWithCategoriesWithInventories.OrderBy(X => X.Quantity);
            if (request.SortBy.ToLower() == "sku")
                ProductsWithCategoriesWithInventories = request.SortDescending ? ProductsWithCategoriesWithInventories.OrderByDescending(X => X.p.Sku) : ProductsWithCategoriesWithInventories.OrderBy(X => X.p.Sku);
            if (request.SortBy.ToLower() == "name")
                ProductsWithCategoriesWithInventories = request.SortDescending ? ProductsWithCategoriesWithInventories.OrderByDescending(X => X.p.Name) : ProductsWithCategoriesWithInventories.OrderBy(X => X.p.Name);
            if (request.SortBy.ToLower() == "price")
                ProductsWithCategoriesWithInventories = request.SortDescending ? ProductsWithCategoriesWithInventories.OrderByDescending(X => X.p.Price) : ProductsWithCategoriesWithInventories.OrderBy(X => X.p.Price);
        }

        var queryCount = await ProductsWithCategoriesWithInventories.CountAsync(cancellationToken);

        var paginatedQuery = ProductsWithCategoriesWithInventories
             .Skip((request.PageSize) * (request.Page - 1))
             .Take(request.PageSize)
             .Select(X => new ProductListItemDto
             (
                 X.p.Id,
                 X.p.Name,
                 X.p.Sku,
                 X.p.Price,
                 X.p.Supplier,
                 X.p.CategoryName,
                 X.p.CategoryId,
                 X.Quantity,
                 X.LowStockThreshold,
                 X.p.Description,
                 X.p.ImageUrl
             ));

        var pagedResult = new PaginatedList<ProductListItemDto>(await paginatedQuery.ToListAsync(cancellationToken), queryCount, request.Page, request.PageSize);

        return Result<PaginatedList<ProductListItemDto>>.Success(pagedResult);
    }
}
