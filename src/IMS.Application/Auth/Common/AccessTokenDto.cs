namespace IMS.Application.Auth.Common
{
    public record AccessTokenDto
    {
        public string Token { get; init; }
        public DateTime ExpiresAt { get; init; }
    }

}
