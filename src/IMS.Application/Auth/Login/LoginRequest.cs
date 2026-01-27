using IMS.Application.Auth.Common;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.Login
{
    public record LoginRequest : IRequest<Result<AuthenticationResponse>>
    {
        public string Email { get; init; }
        public string Password { get; init; }
    }

}

