using GuardianStock.Application.Auth.Common;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.GoogleLogin
{
    public record GoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public required string IdToken { get; init; }
    }
}
