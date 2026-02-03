using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string? Name, string? Description) : IRequest<Result>;
