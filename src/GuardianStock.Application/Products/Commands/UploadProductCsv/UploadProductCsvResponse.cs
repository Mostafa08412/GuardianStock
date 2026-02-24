namespace GuardianStock.Application.Products.Commands.UploadProductCsv
{
    public record UploadProductCsvResponse
    {
        public string jobId { get; init; }
    }
}