using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.DeactivateUser;

public record DeactivateUserCommand(string UserId) : IRequest<Result>;
