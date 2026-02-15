using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Role) : IRequest<Result>;
