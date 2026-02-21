namespace GuardianStock.Infrastructure.HubServices.Settings
{
    public class ImportProductsHubSettings
    {
        public static string SectionName => "ImportProducts";
        public string Status { get; set; } = string.Empty;
        public string OnPreviewReady { get; set; } = string.Empty;
        public string OnImportCompleted { get; set; } = string.Empty;
        public string OnJobFailed { get; set; } = string.Empty;
        public string OnProgress { get; set; } = string.Empty;
    }
}
