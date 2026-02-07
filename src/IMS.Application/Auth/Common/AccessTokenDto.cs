using System.Text.Json.Serialization;

namespace IMS.Application.Auth.Common
{
    public record AccessTokenDto
    {
        public string Token { get; init; }

        [JsonIgnore]
        public DateTime ExpiresAt { get; init; }
    }

}
