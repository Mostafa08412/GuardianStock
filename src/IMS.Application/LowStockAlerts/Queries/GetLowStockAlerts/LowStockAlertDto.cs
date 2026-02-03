namespace IMS.Application.LowStockAlerts.Queries.GetLowStockAlerts;

public record LowStockAlertDto(
    Guid InventoryId,
    Guid ProductId,
    string ProductName,
    string Sku,
    int CurrentStock,
    int Threshold,
    bool IsNotificationSent,
    DateTime TriggeredOn);
