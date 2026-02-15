using IMS.Application.Auth.Common;
using IMS.Application.Contracts.Identity;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResponse>>
    {
        private readonly IIdentityService _identityService;

        public RefreshTokenCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.AuthenticateByRefreshTokenAsync(
           request.refreshToken,
           cancellationToken);

            if (!result.IsSuccess)
                return Result<AuthenticationResponse>.Failure(result.Errors);


            var authData = result.Value!;

            var response = new AuthenticationResponse
            {
                User = new IdentityUserDto
                {
                    Id = authData.UserId,
                    Email = authData.Email,
                    FirstName = authData.FirstName,
                    LastName = authData.LastName,
                    UserName = authData.UserName,
                    Roles = authData.Roles
                },
                AccessToken = new AccessTokenDto
                {
                    Token = authData.AccessToken,
                    ExpiresAt = authData.AccessTokenExpiresAt.ToLocalTime()
                },
                RefreshToken = new RefreshTokenDto
                {
                    Token = authData.RefreshToken,
                    ExpiresAt = authData.RefreshTokenExpiresAt.ToLocalTime()
                }
            };

            return Result<AuthenticationResponse>.Success(response);
        }
    }

}
