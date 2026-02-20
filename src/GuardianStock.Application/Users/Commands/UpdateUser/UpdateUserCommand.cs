using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    string UserId,
    string FirstName,
    string LastName,
    string Role) : IRequest<Result>;
