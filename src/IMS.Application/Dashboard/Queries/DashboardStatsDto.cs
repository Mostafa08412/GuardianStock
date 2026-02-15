namespace IMS.Application.Dashboard.Queries
{
    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal TotalSales { get; set; }
        public int LowStockCount { get; set; }
        public List<RecentTransactionDto> RecentTransactions { get; set; } = new();
    }
}
