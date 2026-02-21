namespace GuardianStock.Application.Contracts.Identity
{
    public sealed class AuthenticationResult
    {
        public Guid UserId { get; init; }
        public string Email { get; init; }
        public string UserName { get; init; }
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public IEnumerable<string> Roles { get; init; }
        public string AccessToken { get; init; }
        public DateTime AccessTokenExpiresAt { get; init; }  // ← UTC DateTime
        public string RefreshToken { get; init; }
        public DateTime RefreshTokenExpiresAt { get; init; }  // ← UTC DateTime
    }
}
