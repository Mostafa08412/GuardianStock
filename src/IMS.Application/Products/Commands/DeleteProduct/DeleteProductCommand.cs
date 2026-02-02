using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : IRequest<Result>;
