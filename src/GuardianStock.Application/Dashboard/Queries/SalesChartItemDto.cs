namespace GuardianStock.Application.Dashboard.Queries
{
    public class SalesChartItemDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal Sales { get; set; }
        public decimal Purchases { get; set; }
    }
}
