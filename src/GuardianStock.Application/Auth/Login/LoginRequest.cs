using GuardianStock.Application.Auth.Common;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.Login
{
    public record LoginRequest : IRequest<Result<AuthenticationResponse>>
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }

}

