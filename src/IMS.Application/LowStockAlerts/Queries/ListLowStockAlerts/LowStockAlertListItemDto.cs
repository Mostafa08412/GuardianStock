namespace IMS.Application.LowStockAlerts.Queries.ListLowStockAlerts;

public record LowStockAlertListItemDto(
    Guid InventoryId,
    Guid ProductId,
    string ProductName,
    string Sku,
    int CurrentStock,
    int Threshold,
    string status,
    bool IsNotificationSent,
    bool isDismissed,
    DateTime TriggeredOn);
