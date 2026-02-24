namespace GuardianStock.API.Extensions
{
    public class CorsSettings
    {
        public const string SectionName = "CorsSettings";
        public string PolicyName { get; init; } = string.Empty;

        public List<string> AllowedOrigins { get; init; } = new List<string>();
    }


}
