using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.LowStockAlerts.Queries.GetStockAlertDetails;

public class GetStockAlertDetailsQueryHandler : IRequestHandler<GetStockAlertDetailsQuery, Result<StockAlertDetailsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetStockAlertDetailsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<StockAlertDetailsDto>> Handle(GetStockAlertDetailsQuery request, CancellationToken cancellationToken)
    {
        var result = await (from inventory in _context.Inventories.AsNoTracking()
                            join product in _context.Products.AsNoTracking() on inventory.ProductId equals product.Id
                            where inventory.Id == request.InventoryId
                            select new
                            {
                                Inventory = inventory,
                                Product = product,
                            }).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return Result<StockAlertDetailsDto>.Failure(Errors.InventoryErrors.NotFound);
        }

        var inventoryEntity = result.Inventory;
        var productEntity = result.Product;

        var transactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.ProductId == productEntity.Id)
            .OrderByDescending(t => t.CreatedOnUTC)
            .Take(5)
            .Select(t => new StockTransactionDto(
                t.Id,
                t.Type.ToString(), // Assuming TransactionType has a decent ToString or I might need to format it
                t.Quantity,
                t.CreatedOnUTC.ToLocalTime(),
                t.TotalAmount
            ))
            .ToListAsync(cancellationToken);

        decimal stockValue = inventoryEntity.Quantity * productEntity.Price;
        int shortageQuantity = inventoryEntity.LowStockThreshold - inventoryEntity.Quantity > 0
            ? inventoryEntity.LowStockThreshold - inventoryEntity.Quantity
            : 0;

        // Determine status string similar to logic in AlertDetails.tsx or Domain
        // Domain has InventoryStatus enum: Critical, Low, Healthy
        // UI shows "Low Stock" badge.
        string status = inventoryEntity.Status.ToString();

        var dto = new StockAlertDetailsDto(
            inventoryEntity.Id,
            productEntity.Id,
            productEntity.Name,
            productEntity.Sku,
            productEntity.Price,
            productEntity.Supplier,
            inventoryEntity.Quantity,
            inventoryEntity.LowStockThreshold,
            shortageQuantity,
            stockValue,
            status,
            inventoryEntity.LowStockAlert?.TriggeredAtUTC.ToLocalTime(),
            inventoryEntity.LowStockAlert?.IsNotificationSent ?? false,
            transactions
        );

        return Result<StockAlertDetailsDto>.Success(dto);
    }
}
