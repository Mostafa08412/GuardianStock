namespace IMS.Application.Auth.Common
{
    public record RefreshTokenDto
    {
        public string Token { get; init; }
        public DateTime ExpiresAt { get; init; }
    }

}
