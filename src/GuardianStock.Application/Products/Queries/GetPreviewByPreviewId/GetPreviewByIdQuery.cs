using GuardianStock.Application.Contracts.CsvFileReader;
using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Products.Queries.GetPreviewByPreviewId
{
    public record GetPreviewByIdQuery(string PreviewId) : IRequest<Result<List<ProductCSVModel>>>;

}
