using GuardianStock.Application.Contracts.Identity;

namespace GuardianStock.Application.Auth.Common
{
    public class AuthenticationResponse
    {

        public IdentityUserDto User { get; set; }

        public AccessTokenDto AccessToken { get; set; }

        public RefreshTokenDto RefreshToken { get; set; }


    }

}
