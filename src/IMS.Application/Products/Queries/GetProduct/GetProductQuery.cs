using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Queries.GetProduct;

public record GetProductQuery(Guid ProductId) : IRequest<Result<ProductDto>>;
