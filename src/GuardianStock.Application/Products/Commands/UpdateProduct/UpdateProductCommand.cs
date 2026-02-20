using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace GuardianStock.Application.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid ProductId,
    string? Name,
    string? Description,
    Guid? CategoryId,
    decimal? Price,
    string? Supplier,
    int? LowStockAlertThreshold,
    IFormFile? Image) : IRequest<Result>;
