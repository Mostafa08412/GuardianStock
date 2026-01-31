using IMS.Domain.Core.Primitives;

namespace IMS.Application.Common.Errors
{
    public static class ApplicationErrors
    {

        public static class CSVFileReaderErrors
        {

            public static Error ProductCsvFileParsingError(int rowNumber) =>
                new Error("CSV.ProductCsvFileParsingError", $"An error has been occured during parsing row number {rowNumber}, please make sure all rows have values for each column.", ErrorType.Validation);

        }
    }
}
