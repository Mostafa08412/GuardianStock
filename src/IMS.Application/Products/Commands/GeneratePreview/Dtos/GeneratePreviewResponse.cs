using IMS.Application.Contracts.CsvFileReader;

namespace IMS.Application.Products.Commands.GeneratePreview.Dtos
{
    public class GeneratePreviewResponse
    {
        public string JobId { get; set; }
        public List<PreviewRowResult> RowResults { get; set; } = new();
        public int SucceededCount => RowResults.Count(X => X.IsValid);
        public int FailedCount => RowResults.Count(X => !X.IsValid);

    }


    public class PreviewRowResult
    {
        public int RowNumber { get; init; }
        public string Name { get; init; }

        public string Description { get; init; }

        public string Category { get; init; }

        public string? CategoryId { get; init; }

        public string Price { get; init; }

        public string Quantity { get; init; }

        public string Supplier { get; init; }
        public string LowStockAlertThreshold { get; init; }

        public bool IsValid { get; init; }

        public List<string> Errors { get; init; } = new();


        public static PreviewRowResult CreateSuccess(ProductCSVModel model, int rowNumber, Guid categoryId)
        {
            var p = model;
            var importProductsRowResult = new PreviewRowResult
            {
                RowNumber = rowNumber,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                Category = p.Category ?? string.Empty,
                CategoryId = categoryId.ToString(),
                Supplier = p.Supplier ?? string.Empty,
                Quantity = p.InitialQuantity,
                LowStockAlertThreshold = p.LowStockAlertThreshold,
                Price = p.Price,
                IsValid = true


            };
            return importProductsRowResult;
        }

        public static PreviewRowResult CreateFailure(ProductCSVModel model, int rowNumber, IEnumerable<string> errors)
        {
            var p = model;
            var importProductsRowResult = new PreviewRowResult
            {
                RowNumber = rowNumber,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                Category = p.Category ?? string.Empty,
                Supplier = p.Supplier ?? string.Empty,
                Quantity = p.InitialQuantity,
                LowStockAlertThreshold = p.LowStockAlertThreshold,
                Price = p.Price,
                IsValid = false


            };
            importProductsRowResult.Errors.AddRange(errors);
            return importProductsRowResult;
        }

        public static PreviewRowResult CreateFailure(ProductCSVModel model, int rowNumber, string error)
        {
            var p = model;
            var importProductsRowResult = new PreviewRowResult
            {
                RowNumber = rowNumber,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                Category = p.Category ?? string.Empty,
                Supplier = p.Supplier ?? string.Empty,
                Quantity = p.InitialQuantity,
                LowStockAlertThreshold = p.LowStockAlertThreshold,
                Price = p.Price,
                IsValid = false


            };
            importProductsRowResult.Errors.Add(error);
            return importProductsRowResult;
        }
    }
}
