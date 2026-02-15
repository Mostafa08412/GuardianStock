using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    string UserId,
    string FirstName,
    string LastName,
    string Role) : IRequest<Result>;
