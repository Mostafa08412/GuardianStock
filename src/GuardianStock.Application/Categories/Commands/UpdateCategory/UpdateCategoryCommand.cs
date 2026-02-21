using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(Guid Id, string? Name, string? Description) : IRequest<Result>;
