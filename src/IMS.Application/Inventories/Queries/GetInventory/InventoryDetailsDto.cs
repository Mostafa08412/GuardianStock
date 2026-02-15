using IMS.Domain.Transactions;

namespace IMS.Application.Inventories.Queries.GetInventory
{
    public class InventoryDetailsDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductSku { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string Supplier { get; set; } = string.Empty;

        public int Stock { get; set; }
        public int LowStockThreshold { get; set; }
        public string StockStatus { get; set; } = string.Empty;
        public decimal StockValue { get; set; }
        public int ShortageQuantity { get; set; }

        public DateTime? AlertTriggeredAt { get; set; }
        public bool IsNotificationSent { get; set; }
        public bool IsDismissed { get; set; }
        public DateTime? DismissedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public List<InventoryTransactionDto> RecentTransactions { get; set; } = [];
    }

    public class InventoryTransactionDto
    {
        public Guid Id { get; set; }
        public TransactionType Type { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime Date { get; set; }
    }
}
