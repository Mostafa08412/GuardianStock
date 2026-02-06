using IMS.Domain.Core.Primitives;

namespace IMS.Application.Common.Errors;

public static class ApplicationErrors
{
    public static class CsvReader
    {
        // General CSV File Errors
        public static Error InvalidFormat => new(
            "CSV.InvalidFormat",
            "Invalid uploaded file format. Only supporting CSV file format.",
            ErrorType.Failure);

        public static Error FileIsRequired => new(
            "CSV.FileIsRequired",
            "The CSV file is required.",
            ErrorType.Validation);

        public static Error FileNotFound => new(
         "CSV.FileNotFound",
         "The CSV file is not found.",
         ErrorType.NotFound);

        public static Error MissingHeaders => new(
            "CSV.MissingHeaders",
            $"The uploaded file is missing required columns.",
            ErrorType.Failure);

        public static Error UnexpectedError => new(
        "CSV.UnexpectedError",
        $"Unexpected error has occurred during reading CSV file.",
        ErrorType.Failure);

        public static readonly Error PreviewExpired = new(
            "CsvReader.PreviewExpired",
            "The preview data has expired or is no longer available. Please upload the file again.",
            ErrorType.NotFound);

        // Product-Specific Row Validation Errors
        public static class Product
        {
            public static Error InvalidPrice => new(
                "CSV.Product.InvalidPrice",
                "Invalid price format.",
                ErrorType.Validation);

            public static Error InvalidInitialStock => new(
                "CSV.Product.InvalidInitialStock",
                "Invalid initial stock format.",
                ErrorType.Validation);

            public static Error InvalidLowStockFormat => new(
                "CSV.Product.InvalidLowStockFormat",
                "Invalid low stock threshold format.",
                ErrorType.Validation);

            public static Error LowStockTooLow(int minThreshold) => new(
                "CSV.Product.LowStockTooLow",
                $"Invalid low stock threshold quantity (minimum is {minThreshold}).",
                ErrorType.Validation);

            public static Error DuplicateProduct => new(
                "CSV.Product.Duplicate",
                "Duplicated product found in the uploaded file.",
                ErrorType.Validation);

            public static Error InvalidCategory => new(
                "CSV.Product.InvalidCategory",
                "The specified category does not exist in the system.",
                ErrorType.Validation);
        }
    }


}