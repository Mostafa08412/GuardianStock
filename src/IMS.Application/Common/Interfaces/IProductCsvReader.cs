using IMS.Application.Contracts.CsvFileReader;
using IMS.Domain.Core.Primitives.Result;
using Microsoft.AspNetCore.Http;

namespace IMS.Application.Common.Interfaces
{
    public interface IProductCsvReader
    {
        Task<Result<List<ProductCSVModel>>> ReadProductCSVFile(IFormFile file);

    }
}
