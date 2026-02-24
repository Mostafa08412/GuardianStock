using System.Text.Json.Serialization;

namespace GuardianStock.Application.Auth.Common
{
    public record RefreshTokenDto
    {
        public string Token { get; init; }

        [JsonIgnore]
        public DateTime ExpiresAt { get; init; }
    }

}
