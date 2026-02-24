using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Products.Queries.GetProductDetails;

public record GetProductDetailsQuery(Guid ProductId) : IRequest<Result<ProductDetailsDto>>;
