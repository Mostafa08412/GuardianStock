namespace GuardianStock.API.Extensions
{
    public class DatabaseInitializationSettings
    {
        public const string SectionName = "InitializeDatabase";
        public bool ResetDatabase { get; init; } = false;
        public bool InitializeDatabase { get; init; } = false;
        public bool SeedData { get; init; } = false;
    }


}
