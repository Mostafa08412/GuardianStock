using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Description) : IRequest<Result<Guid>>;
