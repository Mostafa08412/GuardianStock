using IMS.Domain.Transactions;

namespace IMS.Application.Products.Queries.GetProduct;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public string Description { get; init; }
    public string Supplier { get; init; }
    public string CategoryName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdatedAt { get; init; }
    public decimal TotalValue => StockQuantity * Price;

    public string StockStatus
    {
        get
        {

            if (StockQuantity == 0)
                return "critical";

            decimal ratio = (decimal)StockQuantity / LowStockThreshold;

            return ratio switch
            {
                <= 0.3m => "critical",
                > 0.3m and < 1.0m => "low",
                _ => "normal"

            };

        }
    }
    public string Sku { get; init; }
    public decimal Price { get; init; }
    public int StockQuantity { get; init; }
    public int LowStockThreshold { get; init; }

    public decimal TwoMonthsAgoSales { get; init; }
    public decimal LastMonthSales { get; init; }
    public decimal CurrentMonthSales { get; init; }

    public decimal AvgReStockTime { get; set; }

    public DateTime? LastRestocked { get; init; }

    public List<StockActivity> RecentActivities { get; init; } = new();

}
public class StockActivity
{
    public TransactionType Type { get; init; }


    public DateTime Date { get; init; }


}