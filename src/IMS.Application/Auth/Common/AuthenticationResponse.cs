using IMS.Application.Contracts.Identity;

namespace IMS.Application.Auth.Common
{
    public class AuthenticationResponse
    {

        public IdentityUserDto User { get; set; }

        public AccessTokenDto AccessToken { get; set; }

        public RefreshTokenDto RefreshToken { get; set; }


    }

}
