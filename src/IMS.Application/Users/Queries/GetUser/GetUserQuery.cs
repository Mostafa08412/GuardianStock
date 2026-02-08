using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Queries.GetUser;

public record GetUserQuery(string UserId) : IRequest<Result<UserDto>>;
