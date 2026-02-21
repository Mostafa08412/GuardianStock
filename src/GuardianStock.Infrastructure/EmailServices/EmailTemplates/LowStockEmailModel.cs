namespace GuardianStock.Infrastructure.EmailServices.EmailTemplates
{
    public class LowStockEmailModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int CurrentQuantity { get; set; }
        public int Threshold { get; set; }
        public string DashboardUrl { get; set; } = string.Empty;
        public DateTime AlertTime { get; set; }
    }
}
