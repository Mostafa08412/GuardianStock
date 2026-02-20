using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.Logout
{
    public record LogoutCommand : IRequest<Result> { }
}
