namespace IMS.Application.Dashboard
{
    public class DashboardStatsDto
    {
        public int TotalProducts { get; set; }
        public decimal TotalStockValue { get; set; }
        public decimal TotalSales { get; set; }
        public int LowStockCount { get; set; }
        public List<TransactionDto> RecentTransactions { get; set; } = new();
    }
}
