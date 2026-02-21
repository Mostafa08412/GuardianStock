using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Description) : IRequest<Result<Guid>>;
