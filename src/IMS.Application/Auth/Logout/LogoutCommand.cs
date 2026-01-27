using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.Logout
{
    public record LogoutCommand : IRequest<Result> { }
}
