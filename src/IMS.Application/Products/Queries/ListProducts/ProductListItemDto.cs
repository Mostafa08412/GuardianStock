namespace IMS.Application.Products.Queries.ListProducts;

public record ProductListItemDto(
    Guid Id,
    string Name,
    string Sku,
    decimal Price,
    string Supplier,
    string CategoryName,
    Guid CategoryId,
    int CurrentStock,
    int LowStockThreshold,
    string Description);
