using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace IMS.Application.Dashboard.GetDashboardStats
{

    public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardDto>>
    {

        private readonly IApplicationDbContext context;
        private readonly IDateTime dateTime;


        public GetDashboardStatsQueryHandler(IApplicationDbContext dbcontext, IDateTime dateTime)
        {
            this.dateTime = dateTime;
            context = dbcontext;
        }

        public async Task<Result<DashboardDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
        {
            var dashboard = new DashboardDto();


            dashboard.Stats.TotalProducts = await context.Products.AsNoTracking().CountAsync(cancellationToken);

            dashboard.Stats.TotalStockValue = await (from product in context.Products.AsNoTracking()
                                                     join inventory in context.Inventories.AsNoTracking()
                                                     on product.Id equals inventory.ProductId
                                                     select new
                                                     {
                                                         UnitPrice = product.Price,
                                                         Stock = inventory.Quantity
                                                     }).SumAsync(X => X.Stock * X.UnitPrice, cancellationToken);

            dashboard.Stats.TotalSales = await context.Transactions.Where(X => X.Type == TransactionType.Sale).SumAsync(X => X.Quantity * X.UnitPrice, cancellationToken);

            dashboard.Stats.LowStockCount = await context.Inventories.Where(X => X.Quantity < X.LowStockThreshold).CountAsync(cancellationToken);




            dashboard.SalesChartData = await
                                        (context.Transactions.AsNoTracking()
                                        .OrderBy(X => X.CreatedOnUTC.Year).ThenBy(X => X.CreatedOnUTC.Month)
                                        .GroupBy(X => new { X.CreatedOnUTC.Year, X.CreatedOnUTC.Month })
                                        .Select(X => new SalesChartItemDto
                                        {
                                            Month = X.Key.Month,
                                            Year = X.Key.Year,
                                            Purchases = X.Where(X => X.Type == TransactionType.Purchase).Sum(X => X.Quantity * X.UnitPrice),
                                            Sales = X.Where(X => X.Type == TransactionType.Sale).Sum(X => X.Quantity * X.UnitPrice)

                                        }))
                                        .OrderBy(X => X.Year).ThenBy(X => X.Month)

                                        .ToListAsync(cancellationToken);



            dashboard.CategoryDistribution = await (from category in context.Categories.AsNoTracking()
                                                    join product in context.Products.AsNoTracking()
                                                    on category.Id equals product.CategoryId
                                                    select new
                                                    {
                                                        CategoryName = category.Name,
                                                        ProductId = product.Id
                                                    }
                                                    ).GroupBy(X => X.CategoryName)
                                                    .Where(X => X.Count() > 0)
                                                    .Select(X => new CategoryChartItemDto
                                                    {
                                                        Name = X.Key,
                                                        Value = X.Count()
                                                    }).ToListAsync(cancellationToken);


            dashboard.LowStockInventories = await (from inventory in context.Inventories.AsNoTracking()
                                                   join product in context.Products.AsNoTracking()
                                                   on inventory.Id equals product.Id
                                                   select new LowStockInventoryDto
                                                   {
                                                       InventoryId = inventory.Id,
                                                       ProductName = product.Name,
                                                       Status =
                                                       (((decimal)inventory.Quantity / inventory.LowStockThreshold) > 1
                                                       ? "Normal"
                                                       : ((decimal)inventory.Quantity / inventory.LowStockThreshold) < 0.3m
                                                            ? "Critical"
                                                           : "Low"
                                                        ),
                                                       Stock = inventory.Quantity,
                                                       Threshold = inventory.LowStockThreshold
                                                   }

                                                    ).Take(3).ToListAsync(cancellationToken);



            var recentTransaction = (await (from transaction in context.Transactions.AsNoTracking().OrderBy(X => X.CreatedOnUTC)
                                            join product in context.Products.AsNoTracking()
                                            on transaction.ProductId equals product.Id
                                            join inventory in context.Inventories.AsNoTracking()
                                            on transaction.ProductId equals inventory.ProductId
                                            join user in context.BusinessUsers.AsNoTracking().DefaultIfEmpty()
                                            on transaction.CreatedBy equals user.Id
                                            select new RecentTransactionDto
                                            {

                                                Id = transaction.Id.ToString(),
                                                Date = transaction.CreatedOnUTC,
                                                ProductName = product.Name,
                                                Quantity = transaction.Quantity,
                                                Type = transaction.Type.ToString(),
                                                TotalAmount = transaction.Quantity * transaction.UnitPrice,
                                                UserName = user.FirstName + " " + user.LastName,


                                            }


                ).Take(5).ToListAsync(cancellationToken));

            dashboard.Stats.RecentTransactions = recentTransaction.Select(X => new RecentTransactionDto
            {
                Date = X.Date.ToLocalTime(),
                Id = X.Id,
                ProductName = X.ProductName,
                Quantity = X.Quantity,
                TotalAmount = X.TotalAmount,
                UserName = X.UserName,
                Type = X.Type
            }).ToList();



            dashboard.TopProducts = await ((from product in context.Products.AsNoTracking()
                                            join transaction in context.Transactions.AsNoTracking().Where(X => X.CreatedOnUTC.Year == dateTime.UTCNow.Year && X.CreatedOnUTC.Month == dateTime.UTCNow.Month)
                                            on product.Id equals transaction.ProductId
                                            select new
                                            {
                                                ProductName = product.Name,
                                                ProductId = product.Id,
                                                Transaction = transaction
                                            })
                                     .GroupBy(X => new { X.ProductId, X.ProductName }, X => X.Transaction)

                                     .Select(X => new TopProductItemDto
                                     {
                                         Name = X.Key.ProductName,
                                         Revenue = X.Where(X => X.Type == TransactionType.Sale).Sum(X => X.Quantity * X.UnitPrice),
                                         Sales = X.Where(X => X.Type == TransactionType.Sale).Sum(X => X.Quantity)
                                     })
                                     .OrderByDescending(X => X.Revenue)


                                     ).Take(10).ToListAsync(cancellationToken);


            return Result<DashboardDto>.Success(dashboard);
        }


    }
}
