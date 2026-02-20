using GuardianStock.Application.Categories.Queries;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid Id) : IRequest<Result<CategoryDetails>>;
