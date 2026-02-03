using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Categories.Queries.GetCategory;

public record GetCategoryQuery(Guid Id) : IRequest<Result<CategoryDto>>;
