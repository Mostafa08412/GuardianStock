using IMS.Application.Auth.Common;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.RefreshToken
{
    public record RefreshTokenCommand : IRequest<Result<AuthenticationResponse>>
    {
        public string refreshToken { get; init; }
    }

}
