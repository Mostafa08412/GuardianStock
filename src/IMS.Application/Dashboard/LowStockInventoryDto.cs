namespace IMS.Application.Dashboard
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
