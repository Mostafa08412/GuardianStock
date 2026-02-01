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
