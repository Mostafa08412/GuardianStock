using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Queries.GetUser;

public record GetUserDetailsQuery(string UserId) : IRequest<Result<UserDetailsDto>>;
