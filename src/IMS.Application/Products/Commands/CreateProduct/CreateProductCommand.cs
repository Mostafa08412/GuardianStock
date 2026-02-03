using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Supplier,
    Guid CategoryId,
    int InitialQuantity,
    int LowStockThreshold) : IRequest<Result<Guid>>;
