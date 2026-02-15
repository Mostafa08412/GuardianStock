using IMS.Application.Auth.Common;
using IMS.Application.Contracts.Identity;

namespace IMS.Application.Common.Mappers
{
    public static class MapperExtensions
    {


        public static AuthenticationResponse MapToAuthenticationResponse(this AuthenticationResult result)
        {
            return new AuthenticationResponse
            {
                User = new IdentityUserDto
                {
                    Id = result.UserId,
                    Email = result.Email,
                    FirstName = result.FirstName,
                    LastName = result.LastName,
                    UserName = result.UserName,
                    Roles = result.Roles
                },
                AccessToken = new AccessTokenDto
                {
                    Token = result.AccessToken,
                    ExpiresAt = result.AccessTokenExpiresAt.ToLocalTime()
                },
                RefreshToken = new RefreshTokenDto
                {
                    Token = result.RefreshToken,
                    ExpiresAt = result.RefreshTokenExpiresAt.ToLocalTime()
                }
            };

        }
    }
}
