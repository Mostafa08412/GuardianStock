using GuardianStock.Application.Contracts.CsvFileReader;
using GuardianStock.Domain.Core.Primitives.Result;

namespace GuardianStock.Application.Common.Interfaces
{
    public interface IProductCsvReader
    {
        Task<Result<List<ProductCSVModel>>> ReadProductCSVFile(string filePath);

    }
}
