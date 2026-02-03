namespace IMS.Application.Categories.Queries;

public record CategoryDto(
    Guid Id,
    string Name,
    string Description,
    int ProductCount);
