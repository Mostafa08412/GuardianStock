using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Transactions;
using GuardianStock.Domain.Core.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GuardianStock.Application.Products.Queries.GetProductDetails;

public class GetProductDetailsQueryHandler : IRequestHandler<GetProductDetailsQuery, Result<ProductDetailsDto>>
{
    private readonly IApplicationDbContext context;
    private readonly IDateTime date;

    public GetProductDetailsQueryHandler(IApplicationDbContext context, IDateTime date)
    {
        this.context = context;
        this.date = date;
    }

    public async Task<Result<ProductDetailsDto>> Handle(GetProductDetailsQuery request, CancellationToken cancellationToken)
    {

        var productData = await (from product in context.Products.AsNoTracking()
                                 where product.Id == request.ProductId
                                 join category in context.Categories
                                      on product.CategoryId equals category.Id
                                 join inventory in context.Inventories
                                      on product.Id equals inventory.ProductId
                                 select new
                                 {
                                     Id = product.Id,
                                     Name = product.Name,
                                     Price = product.Price,
                                     Description = product.Description,
                                     CategoryName = category.Name,
                                     CategoryId = category.Id,
                                     Sku = product.Sku,
                                     StockQuantity = inventory.Quantity,
                                     LowStockThreshold = inventory.LowStockThreshold,
                                     Supplier = product.Supplier,
                                     ImageUrl = product.ImageUrl,
                                     CreatedAt = product.CreatedOnUTC.ToLocalTime(),
                                     LastUpdatedAt = product.UpdatedOnUTC.ToLocalTime()
                                 }).FirstOrDefaultAsync(cancellationToken);

        if (productData is null)
            return Result<ProductDetailsDto>.Failure(Errors.ProductErrors.ProductNotFound);


        DateTime previousMonth = date.UTCNow.AddMonths(-2);

        var sales = await context.Transactions.AsNoTracking()
            .Where(X => X.ProductId == request.ProductId && X.CreatedOnUTC >= previousMonth && X.Type == TransactionType.Sale)
            .GroupBy(X => X.CreatedOnUTC.Month)
            .Select(g => new { Month = g.Key, Sales = g.Sum(X => X.Quantity * X.UnitPrice) })
            .ToListAsync(cancellationToken);

        var curentMonthSales = sales.FirstOrDefault(X => X.Month == date.UTCNow.Month)?.Sales ?? 0;
        var prevMonthSales = sales.FirstOrDefault(X => X.Month == date.UTCNow.AddMonths(-1).Month)?.Sales ?? 0;
        var twoMonthsAgoSales = sales.FirstOrDefault(X => X.Month == date.UTCNow.AddMonths(-2).Month)?.Sales ?? 0;

        var ratio = twoMonthsAgoSales == 0 ? 0 : (decimal)prevMonthSales / twoMonthsAgoSales;

        var lastRestock = await context.Transactions.AsNoTracking()
           .Where(X => X.ProductId == request.ProductId && X.CreatedOnUTC >= previousMonth && X.Type == TransactionType.Purchase)
           .OrderByDescending(X => X.CreatedOnUTC)
           .Select(X => X.CreatedOnUTC)
           .FirstOrDefaultAsync(cancellationToken);

        if (lastRestock == default)
            lastRestock = productData.CreatedAt;


        var recentActivity = await context.Transactions.AsNoTracking()
            .Where(X => X.ProductId == request.ProductId)
            .OrderByDescending(X => X.CreatedOnUTC)
            .Take(5)
            .Select(X => new StockActivity
            {
                Date = X.CreatedOnUTC,
                Type = X.Type,
                Quantity = X.Quantity

            })
            .ToListAsync(cancellationToken);

        int avgRestock = (await context.DB.SqlQuery<int>($@"
            WITH CTE AS 
            (
            SELECT 
            DATEDIFF(DAY,LAG(CreatedOnUTC) over (order by CreatedOnUTC asc),CreatedOnUTC) as daysDiff
            FROM Transactions AS T
            WHERE T.ProductId = {request.ProductId} and T.Type = 1
            )

            select ISNULL(AVG(daysDiff),0)
            from CTE;

          ").ToListAsync(cancellationToken)).FirstOrDefault();

        var stockHistory = from inventory in context.Inventories.AsNoTracking().Where(X => X.ProductId == request.ProductId)
                           join historyItem in context.StockHistories.AsNoTracking()
                           on inventory.Id equals historyItem.InventoryId
                           select new
                           {
                               Quantity = historyItem.CurrentStock,
                               Date = historyItem.TimestampUTC
                           };


        var stockhistoryList = await stockHistory
            .GroupBy(X => new { X.Date.Year, X.Date.Month })
            .Select(X => new StockHistoryDTO
            {
                Year = X.Key.Year,
                Month = X.Key.Month,
                Quantity = X.OrderByDescending(X => X.Date).FirstOrDefault().Quantity

            }).ToListAsync(cancellationToken);

        var result = new ProductDetailsDto
        {
            Id = productData.Id,
            Name = productData.Name,
            Price = productData.Price,
            Description = productData.Description,
            Sku = productData.Sku,
            Supplier = productData.Supplier,
            StockQuantity = productData.StockQuantity,
            LowStockThreshold = productData.LowStockThreshold,
            CategoryName = productData.CategoryName,
            ImageUrl = productData.ImageUrl,
            CategoryId = productData.CategoryId.ToString(),
            UpdatedAt = productData.LastUpdatedAt,
            CreatedAt = productData.CreatedAt,
            RecentActivities = recentActivity,
            AvgReStockTime = avgRestock,
            CurrentMonthSales = curentMonthSales,
            LastMonthSales = prevMonthSales,
            TwoMonthsAgoSales = twoMonthsAgoSales,
            LastRestocked = lastRestock,
            StockHistory = stockhistoryList
        };
        return Result<ProductDetailsDto>.Success(result);
    }



}
