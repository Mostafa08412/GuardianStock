namespace GuardianStock.Infrastructure.EmailServices.EmailTemplates
{
    public class LowStockEmailModel
    {
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public int CurrentQuantity { get; set; }
        public int Threshold { get; set; }
        public string DashboardUrl { get; set; }
        public DateTime AlertTime { get; set; }
    }
}
