using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Users.Queries.GetUser;

public record GetUserDetailsQuery(string UserId) : IRequest<Result<UserDetailsDto>>;
