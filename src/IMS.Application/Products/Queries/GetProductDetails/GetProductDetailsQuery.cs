using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Queries.GetProductDetails;

public record GetProductDetailsQuery(Guid ProductId) : IRequest<Result<ProductDetailsDto>>;
