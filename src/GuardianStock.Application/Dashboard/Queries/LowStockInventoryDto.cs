namespace GuardianStock.Application.Dashboard.Queries
{
    public class LowStockInventoryDto
    {
        public Guid InventoryId { get; set; }
        public string ProductName { get; set; }
        public string Status { get; set; }

        public int Stock { get; set; }

        public int Threshold { get; set; }
    }
}
