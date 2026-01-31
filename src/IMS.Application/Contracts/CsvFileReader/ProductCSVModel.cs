namespace IMS.Application.Contracts.CsvFileReader
{
    public class ProductCSVModel
    {
        public string? Name { get; init; }

        public string? Description { get; init; }

        public decimal Price { get; init; }

        public int InitialQuantity { get; init; }

        public int LowStockAlertThreshold { get; init; }

        public string? Category { get; init; }

        public string? Supplier { get; init; }
    }
}
