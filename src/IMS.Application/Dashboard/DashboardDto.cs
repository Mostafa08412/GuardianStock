namespace IMS.Application.Dashboard
{
    public class DashboardDto
    {
        public DashboardStatsDto Stats { get; set; } = new();
        public List<SalesChartItemDto> SalesChartData { get; set; } = new();
        public List<CategoryChartItemDto> CategoryDistribution { get; set; } = new();
        public List<TopProductItemDto> TopProducts { get; set; } = new();
        public List<LowStockInventoryDto> LowStockInventories { get; set; } = new();
    }
}
