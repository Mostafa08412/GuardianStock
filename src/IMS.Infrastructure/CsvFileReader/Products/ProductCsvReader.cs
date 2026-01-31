using CsvHelper;
using CsvHelper.Configuration;
using IMS.Application.Common.Errors;
using IMS.Application.Common.Interfaces;
using IMS.Application.Contracts.CsvFileReader;
using IMS.Domain.Core.Primitives.Result;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace IMS.Infrastructure.CsvFileReader.Products
{
    public class ProductCsvReader : IProductCsvReader
    {
        public async Task<Result<List<ProductCSVModel>>> ReadProductCSVFile(IFormFile file)
        {
            if (file is null)
                return Result<List<ProductCSVModel>>.Failure(new Domain.Core.Primitives.Error("CSV.FileIsRequired", "The csv file is required.", Domain.Core.Primitives.ErrorType.Validation));

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
                return Result<List<ProductCSVModel>>.Failure(new Domain.Core.Primitives.Error("CSV.InvalidFormat", "Invalid uploaded file format. only supporting csv file format", Domain.Core.Primitives.ErrorType.Validation));

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
                                Price = csvReader.GetField<decimal>("Price"),
                                InitialQuantity = csvReader.GetField<int>("InitialQuantity"),
                                Category = csvReader.GetField<string>("Category"),
                                Supplier = csvReader.GetField<string>("Supplier"),
                            });

                        }
                        catch (Exception)
                        {
                            return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CSVFileReaderErrors.ProductCsvFileParsingError(csvReader.Context.Parser.Row));

                        }


                    }

                }
            }

            return Result<List<ProductCSVModel>>.Success(list);
        }
    }
}
