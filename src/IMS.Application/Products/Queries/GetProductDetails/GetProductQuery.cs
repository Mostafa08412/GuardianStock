using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Queries.GetProductDetails;

public record GetProductQuery(Guid ProductId) : IRequest<Result<ProductDto>>;
