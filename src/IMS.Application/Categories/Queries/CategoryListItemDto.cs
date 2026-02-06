namespace IMS.Application.Categories.Queries;

public record CategoryListItemDto(
    Guid Id,
    string Name,
    string Description,
    int ProductCount);


public record CategoryDetails(
    Guid Id,
    string Name,
    string Description,
    int ProductsCount,
    decimal TotalValue,
    decimal AveragePrice,
    decimal TotalStock
    );
