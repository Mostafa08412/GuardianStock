using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Application.Inventories.Queries.GetInventory;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, Result<InventoryDetailsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetInventoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<InventoryDetailsDto>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var result = await (from inventory in _context.Inventories.AsNoTracking().Where(x => x.Id == request.InventoryId)
                            join product in _context.Products.AsNoTracking() on inventory.ProductId equals product.Id
                            select new
                            {
                                Inventory = inventory,
                                Product = product,
                            }).FirstOrDefaultAsync(cancellationToken);

        if (result is null)
        {
            return Result<InventoryDetailsDto>.Failure(Errors.InventoryErrors.NotFound);
        }

        var inventoryEntity = result.Inventory;
        var productEntity = result.Product;

        var recentTransactions = await _context.Transactions
            .AsNoTracking()
            .Where(t => t.ProductId == productEntity.Id)
            .OrderByDescending(t => t.CreatedOnUTC)
            .Take(5)
            .Select(t => new InventoryTransactionDto
            {
                Id = t.Id,
                Type = t.Type,
                Quantity = t.Quantity,
                TotalAmount = t.TotalAmount,
                Date = t.CreatedOnUTC.ToLocalTime()
            })
            .ToListAsync(cancellationToken);

        int shortageQuantity = inventoryEntity.LowStockThreshold - inventoryEntity.Quantity > 0
            ? inventoryEntity.LowStockThreshold - inventoryEntity.Quantity
            : 0;

        var dto = new InventoryDetailsDto
        {
            Id = inventoryEntity.Id,
            ProductId = productEntity.Id,
            ProductName = productEntity.Name,
            ProductSku = productEntity.Sku,
            ProductPrice = productEntity.Price,
            Supplier = productEntity.Supplier,
            Stock = inventoryEntity.Quantity,
            LowStockThreshold = inventoryEntity.LowStockThreshold,
            StockStatus = inventoryEntity.Status.ToString(),
            StockValue = inventoryEntity.Quantity * productEntity.Price,
            ShortageQuantity = shortageQuantity,
            AlertTriggeredAt = inventoryEntity.LowStockAlert?.TriggeredAtUTC.ToLocalTime(),
            IsNotificationSent = inventoryEntity.LowStockAlert?.IsNotificationSent ?? false,
            IsDismissed = inventoryEntity.LowStockAlert?.DismissedAt != null,
            DismissedAt = inventoryEntity.LowStockAlert?.DismissedAt != null
                ? inventoryEntity.LowStockAlert.DismissedAt.Value.ToLocalTime()
                : null,
            CreatedAt = inventoryEntity.CreatedOnUTC.ToLocalTime(),
            LastUpdatedAt = inventoryEntity.UpdatedOnUTC.ToLocalTime(),
            RecentTransactions = recentTransactions
        };

        return Result<InventoryDetailsDto>.Success(dto);
    }
}
