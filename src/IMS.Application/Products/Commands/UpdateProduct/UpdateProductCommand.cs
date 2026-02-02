using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? Description,
    Guid? CategoryId,
    decimal? Price,
    string? Supplier,
    int? LowStockAlertThreshold) : IRequest<Result>;
