using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Products.Queries.GetProductImage;

public record GetProductImageQuery(string ImageName) : IRequest<Result<ProductImageDto>>;
