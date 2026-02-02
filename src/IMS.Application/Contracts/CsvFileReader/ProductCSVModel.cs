namespace IMS.Application.Contracts.CsvFileReader
{
    public class ProductCSVModel
    {
        public string? Name { get; init; }

        public string? Description { get; init; }

        public string? Price { get; init; }

        public string? InitialQuantity { get; init; }

        public string? LowStockAlertThreshold { get; init; }

        public string? Category { get; init; }

        public string? Supplier { get; init; }
    }
}
