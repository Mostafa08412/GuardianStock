using IMS.Infrastructure.Persistence.Identity;

namespace IMS.Infrastructure.Tokens
{
    public class RefreshToken
    {
        private RefreshToken(string userId, string token, DateTime expiresAtUTC)
        {
            UserId = userId;
            Token = token;
            ExpiresAtUTC = expiresAtUTC;
        }

        protected RefreshToken()
        {

        }


        public string UserId { get; init; }

        public ApplicationUser User { get; init; }

        public string Token { get; init; }

        public DateTime CreatedAtUTC { get; init; } = DateTime.UtcNow;
        public DateTime? RevokedAtUTC { get; private set; }
        public DateTime ExpiresAtUTC { get; init; }

        public bool IsRevoked => RevokedAtUTC != null;
        public bool IsExpired => ExpiresAtUTC <= DateTime.UtcNow;
        public bool IsActive => !IsExpired && !IsRevoked;


        public static RefreshToken Create(string userId, string token, DateTime expiresAtUTC)
        {
            return new RefreshToken(userId, token, expiresAtUTC);
        }


        public void Revoke(DateTime dateTime)
        {
            this.RevokedAtUTC = dateTime;
        }
    }
}
