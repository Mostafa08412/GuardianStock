using IMS.Infrastructure.Persistence.Identity;
using System.Security.Claims;

namespace IMS.Infrastructure.Tokens
{
    public interface ITokenService
    {


        (string, DateTime) GenerateAccessToken(ApplicationUser user, IList<string> roles, IList<Claim>? additionalClaims = null);
        (string, DateTime) GenerateRefreshToken();

    }
}

