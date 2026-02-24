using CsvHelper;
using CsvHelper.Configuration;
using GuardianStock.Application.Common.Errors;
using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Application.Contracts.CsvFileReader;
using GuardianStock.Domain.Core.Primitives.Result;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace GuardianStock.Infrastructure.CsvFileReader.Products
{
    public class ProductCsvReader : IProductCsvReader
    {
        private readonly ILogger<ProductCsvReader> _logger;

        public ProductCsvReader(ILogger<ProductCsvReader> logger)
        {
            _logger = logger;
        }

        public async Task<Result<List<ProductCSVModel>>> ReadProductCSVFile(string filePath)
        {

            _logger.LogInformation("Starting CSV parsing for file: {FilePath}", filePath);

            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.LogWarning("CSV parsing failed: File path is null or empty.");
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.FileIsRequired);
            }

            if (!File.Exists(filePath))
            {
                _logger.LogWarning("CSV parsing failed: File not found at {FilePath}", filePath);
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.FileNotFound);
            }

            if (Path.GetExtension(filePath).ToLower() != ".csv")
            {
                _logger.LogWarning("CSV parsing failed: Invalid extension for file {FilePath}", filePath);
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.InvalidFormat);
            }

            var list = new List<ProductCSVModel>();
            int rowCount = 0;

            try
            {
                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
                using var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8);
                using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));

                await csvReader.ReadAsync();
                csvReader.ReadHeader();

                while (await csvReader.ReadAsync())
                {
                    rowCount++;
                    try
                    {
                        list.Add(new ProductCSVModel
                        {
                            Name = csvReader.GetField<string>("Name"),
                            Description = csvReader.GetField<string>("Description"),
                            Price = csvReader.GetField<string>("Price"),
                            InitialQuantity = csvReader.GetField<string>("InitialQuantity"),
                            Category = csvReader.GetField<string>("Category"),
                            Supplier = csvReader.GetField<string>("Supplier"),
                            LowStockAlertThreshold = csvReader.GetField<string>("LowStockAlertThreshold")
                        });
                    }
                    catch (CsvHelperException ex)
                    {
                        _logger.LogError(ex, "Error parsing row {RowNumber} in file {FilePath}", rowCount, filePath);
                        return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.MissingHeaders);

                    }
                }

                _logger.LogInformation("Successfully parsed {Count} rows from {FilePath}", list.Count, filePath);
                return Result<List<ProductCSVModel>>.Success(list);
            }
            catch (HeaderValidationException ex)
            {
                _logger.LogError(ex, "CSV Header Validation failed for {FilePath}. Expected headers not found.", filePath);
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.MissingHeaders);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "An unexpected error occurred while processing CSV: {FilePath}", filePath);
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.UnexpectedError);
            }
        }


        public async Task<Result<List<ProductCSVModel>>> ReadProductCSVFile(IFormFile file)
        {
            if (file is null)
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.FileIsRequired);

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.InvalidFormat);

            var list = new List<ProductCSVModel>();



            using (Stream stream = file.OpenReadStream())
            {
                using (StreamReader streamReader = new StreamReader(stream, System.Text.Encoding.UTF8))
                {

                    using var csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture));
                    await csvReader.ReadAsync();
                    csvReader.ReadHeader();

                    while (await csvReader.ReadAsync())
                    {


                        try
                        {


                            list.Add(new ProductCSVModel
                            {
                                Name = csvReader.GetField<string>("Name"),
                                Description = csvReader.GetField<string>("Description"),
                                Price = csvReader.GetField<string>("Price"),
                                InitialQuantity = csvReader.GetField<string>("InitialQuantity"),
                                Category = csvReader.GetField<string>("Category"),
                                Supplier = csvReader.GetField<string>("Supplier"),
                                LowStockAlertThreshold = csvReader.GetField<string>("LowStockAlertThreshold")
                            });

                        }
                        catch (Exception)
                        {
                            return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.MissingHeaders);

                        }


                    }

                }
            }

            return Result<List<ProductCSVModel>>.Success(list);
        }
    }
}
