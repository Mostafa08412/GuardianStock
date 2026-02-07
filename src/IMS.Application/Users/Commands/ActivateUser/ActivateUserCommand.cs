using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.ActivateUser;

public record ActivateUserCommand(string UserId) : IRequest<Result>;
