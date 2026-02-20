using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(string UserId) : IRequest<Result>;
