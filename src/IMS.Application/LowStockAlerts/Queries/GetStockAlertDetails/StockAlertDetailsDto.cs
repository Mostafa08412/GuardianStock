namespace IMS.Application.LowStockAlerts.Queries.GetStockAlertDetails;

public record StockAlertDetailsDto(
    Guid Id,
    Guid ProductId,
    string ProductName,
    string ProductSku,
    decimal ProductPrice,
    string Supplier,
    int CurrentStock,
    int LowStockThreshold,
    int ShortageQuantity,
    decimal StockValue,
    string Status,
    DateTime? AlertTriggeredAt,
    bool IsNotificationSent,
    List<StockTransactionDto> RecentTransactions,
    bool IsDismissed,
    DateTime? DismissedAt);

public record StockTransactionDto(
    Guid Id,
    string Type,
    int Quantity,
    DateTime Date,
    decimal TotalAmount);
