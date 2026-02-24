namespace GuardianStock.Application.Inventories.Queries.GetStockSummary
{
    public class StockSummaryDto
    {
        public int totalLowStock { get; init; }
        public int totalCriticalStock { get; init; }
        public int totalNormalStock { get; init; }
    }
}
