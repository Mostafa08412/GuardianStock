using IMS.Application.Auth.Common;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.GoogleLogin
{
    public record GoogleLoginCommand : IRequest<Result<AuthenticationResponse>>
    {
        public required string IdToken { get; init; }
    }
}
