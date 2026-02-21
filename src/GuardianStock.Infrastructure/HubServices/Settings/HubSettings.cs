namespace GuardianStock.Infrastructure.HubServices.Settings
{
    public class HubSettings
    {
        public const string SectionName = "HubSettings";
        public ImportProductsHubSettings ImportProducts { get; set; } = new();
    }
}
