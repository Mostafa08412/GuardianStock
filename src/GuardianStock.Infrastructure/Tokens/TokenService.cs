using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Infrastructure.Persistence.Identity;
using GuardianStock.Infrastructure.Tokens.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace GuardianStock.Infrastructure.Tokens
{
    public sealed class TokenService : ITokenService
    {
        private readonly TokenSettings _tokenSettings;
        private readonly IDateTime _dateTime;

        public TokenService(IOptions<TokenSettings> options, IDateTime dateTime)
        {
            _tokenSettings = options.Value;
            _dateTime = dateTime;
        }

        public (string, DateTime) GenerateAccessToken(ApplicationUser user, IList<string> roles, IList<Claim>? additionalClaims = null)
        {
            var claims = BuildClaims(user, roles, additionalClaims);

            var accessToken = BuildAccessToken(claims);

            var expiresUtc = _dateTime.UTCNow.AddMinutes(_tokenSettings.AccessTokenExpiryMinutes);

            return (accessToken, expiresUtc);
        }


        public (string, DateTime) GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);

            var refreshToken = Convert.ToBase64String(randomBytes);

            var expiresUtc = _dateTime.UTCNow.AddMinutes(_tokenSettings.RefreshTokenExpiryMinutes);


            return (refreshToken, expiresUtc);

        }

        #region Private Helper Methods 
        private SymmetricSecurityKey GetSymmetricSecurityKey() =>
       new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));

        private SigningCredentials GetSigningCredentials() =>
            new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);

        private IEnumerable<Claim> BuildClaims(ApplicationUser user, IList<string> roles, IList<Claim>? additionalClaims)
        {
            var claims = new List<Claim>();

            if (additionalClaims != null && additionalClaims.Any())
                claims.AddRange(additionalClaims);

            claims.AddRange(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        });

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            return claims;
        }

        private string BuildAccessToken(IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_tokenSettings.AccessTokenExpiryMinutes),
                signingCredentials: GetSigningCredentials()
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}

