using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
