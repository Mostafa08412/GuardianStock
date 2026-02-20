namespace GuardianStock.Application.Inventories.Queries.ListInventories
{
    public class InventoryListItemDto
    {
        public Guid Id { get; set; } = Guid.Empty;

        public Guid ProductId { get; set; } = Guid.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }


        public decimal Stock { get; set; }
        public int LowStockThreshold { get; set; }
        public string StockStatus { get; set; } = string.Empty;
    }
}
