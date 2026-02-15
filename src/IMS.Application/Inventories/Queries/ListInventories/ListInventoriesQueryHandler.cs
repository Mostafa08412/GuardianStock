using IMS.Application.Common.Interfaces;
using IMS.Application.Common.Models;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Inventories.Queries.ListInventories;

public class ListInventoriesQueryHandler : IRequestHandler<ListInventoriesQuery, Result<PaginatedList<InventoryListItemDto>>>
{
    private readonly IApplicationDbContext _context;

    public ListInventoriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<InventoryListItemDto>>> Handle(ListInventoriesQuery request, CancellationToken cancellationToken)
    {
        var inventoryQuery = _context.Inventories.AsNoTracking().AsQueryable();
        var productQuery = _context.Products.AsNoTracking().AsQueryable();

        var query = inventoryQuery.Join(
            productQuery,
            i => i.ProductId,
            p => p.Id,
            (i, p) => new
            {
                i.Id,
                i.ProductId,
                ProductName = p.Name,
                ProductSku = p.Sku,
                ProductPrice = p.Price,
                Stock = i.Quantity,
                i.LowStockThreshold,
            });

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x =>
                EF.Functions.Like(x.ProductName, $"%{request.SearchTerm}%") ||
                EF.Functions.Like(x.ProductSku, $"%{request.SearchTerm}%"));
        }

        if (!string.IsNullOrWhiteSpace(request.StockStatus))
        {
            var stockStatus = request.StockStatus.ToLower();

            if (stockStatus == "healthy")
            {
                query = query.Where(x => x.Stock > x.LowStockThreshold);
            }
            else if (stockStatus == "low")
            {
                query = query
                    .Where(x => x.LowStockThreshold > 0)
                    .Where(x => (double)x.Stock / x.LowStockThreshold > 0.3 && (double)x.Stock / x.LowStockThreshold < 1);
            }
            else if (stockStatus == "critical")
            {
                query = query
                    .Where(x => x.LowStockThreshold == 0 || (double)x.Stock / x.LowStockThreshold <= 0.3);
            }
        }

        if (request.MinPrice.HasValue)
        {
            query = query.Where(x => x.ProductPrice >= request.MinPrice);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x => x.ProductPrice <= request.MaxPrice);
        }

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            if (request.SortBy.Equals("ProductName", StringComparison.CurrentCultureIgnoreCase))
                query = request.SortDescending ? query.OrderByDescending(x => x.ProductName) : query.OrderBy(x => x.ProductName);
            else if (request.SortBy.Equals("ProductPrice", StringComparison.CurrentCultureIgnoreCase))
                query = request.SortDescending ? query.OrderByDescending(x => x.ProductPrice) : query.OrderBy(x => x.ProductPrice);
            else if (request.SortBy.Equals("Stock", StringComparison.CurrentCultureIgnoreCase))
                query = request.SortDescending ? query.OrderByDescending(x => x.Stock) : query.OrderBy(x => x.Stock);
        }
        else
        {
            query = query.OrderBy(x => x.Id);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.PageSize * (request.Page - 1))
            .Take(request.PageSize)
            .Select(x => new InventoryListItemDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                ProductSku = x.ProductSku,
                ProductPrice = x.ProductPrice,
                Stock = x.Stock,
                LowStockThreshold = x.LowStockThreshold,
                StockStatus = x.LowStockThreshold == 0
                    ? "Critical"
                    : (double)x.Stock / x.LowStockThreshold <= 0.3
                        ? "Critical"
                        : (double)x.Stock / x.LowStockThreshold <= 1.0
                            ? "Low"
                            : "Healthy"
            })
            .ToListAsync(cancellationToken);

        var paginatedList = PaginatedList<InventoryListItemDto>.Create(items, totalCount, request.Page, request.PageSize);

        return Result<PaginatedList<InventoryListItemDto>>.Success(paginatedList);
    }
}
