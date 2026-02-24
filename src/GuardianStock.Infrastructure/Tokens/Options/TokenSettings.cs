using System.ComponentModel.DataAnnotations;

namespace GuardianStock.Infrastructure.Tokens.Options
{
    public sealed class TokenSettings
    {
        public const string SectionName = "TokenSettings";

        [Required]
        public string Issuer { get; init; } = null!;

        [Required]
        public string Audience { get; init; } = null!;

        [Required]
        [MinLength(32)]
        public string SecretKey { get; init; } = null!;

        [Range(1, 1440)]
        public int AccessTokenExpiryMinutes { get; init; }

        [Range(1, 1440)]
        public int RefreshTokenExpiryMinutes { get; init; }
    }
}
