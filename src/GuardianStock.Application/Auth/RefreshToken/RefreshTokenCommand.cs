using GuardianStock.Application.Auth.Common;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.RefreshToken
{
    public record RefreshTokenCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string refreshToken { get; init; }
    }

}
