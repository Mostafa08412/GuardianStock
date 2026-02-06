using IMS.Application.Contracts.CsvFileReader;
using IMS.Domain.Core.Primitives.Result;

namespace IMS.Application.Common.Interfaces
{
    public interface IProductCsvReader
    {
        Task<Result<List<ProductCSVModel>>> ReadProductCSVFile(string filePath);

    }
}
