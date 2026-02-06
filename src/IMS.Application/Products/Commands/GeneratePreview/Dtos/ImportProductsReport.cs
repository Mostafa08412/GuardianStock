using IMS.Application.Contracts.CsvFileReader;

namespace IMS.Application.Products.Commands.GeneratePreview.Dtos
{
    public class ImportProductsReport
    {
        public List<ImportProductsRowResult> importProductsRowResults { get; set; } = new();
        public int Succeeded => importProductsRowResults.Count(X => X.IsValid);
        public int Failed => importProductsRowResults.Count(X => !X.IsValid);
        public int TotalRows => importProductsRowResults.Count();


    }


    public class ImportProductsRowResult
    {
        public int RowNumber { get; init; }
        public string Name { get; init; }

        public string Description { get; init; }

        public string Category { get; init; }

        public string Price { get; init; }

        public string InitialQuantity { get; init; }

        public string Supplier { get; init; }

        public bool IsValid { get; init; }
        public string LowStockAlertThreshold { get; init; }

        public List<string> Errors { get; init; } = new();


        public static ImportProductsRowResult CreateSuccess(ProductCSVModel model, int rowNumber)
        {
            var p = model;
            var importProductsRowResult = new ImportProductsRowResult
            {
                RowNumber = rowNumber,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                Category = p.Category ?? string.Empty,
                Supplier = p.Supplier ?? string.Empty,
                InitialQuantity = p.InitialQuantity,
                LowStockAlertThreshold = p.LowStockAlertThreshold,
                Price = p.Price,
                IsValid = true


            };
            return importProductsRowResult;
        }

        public static ImportProductsRowResult CreateFailure(ProductCSVModel model, int rowNumber, IEnumerable<string> errors)
        {
            var p = model;
            var importProductsRowResult = new ImportProductsRowResult
            {
                RowNumber = rowNumber,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                Category = p.Category ?? string.Empty,
                Supplier = p.Supplier ?? string.Empty,
                InitialQuantity = p.InitialQuantity,
                LowStockAlertThreshold = p.LowStockAlertThreshold,
                Price = p.Price,
                IsValid = false


            };
            importProductsRowResult.Errors.AddRange(errors);
            return importProductsRowResult;
        }

        public static ImportProductsRowResult CreateFailure(ProductCSVModel model, int rowNumber, string error)
        {
            var p = model;
            var importProductsRowResult = new ImportProductsRowResult
            {
                RowNumber = rowNumber,
                Name = p.Name ?? string.Empty,
                Description = p.Description ?? string.Empty,
                Category = p.Category ?? string.Empty,
                Supplier = p.Supplier ?? string.Empty,
                InitialQuantity = p.InitialQuantity,
                LowStockAlertThreshold = p.LowStockAlertThreshold,
                Price = p.Price,
                IsValid = false


            };
            importProductsRowResult.Errors.Add(error);
            return importProductsRowResult;
        }
    }
}
