using GuardianStock.Application.Products.Queries.ListProducts;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GuardianStock.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Supplier,
    Guid CategoryId,
    int InitialQuantity,
    int LowStockThreshold,
    IFormFile? Image) : IRequest<Result<ProductListItemDto>>;
