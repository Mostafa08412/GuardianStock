using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(string UserId) : IRequest<Result>;
