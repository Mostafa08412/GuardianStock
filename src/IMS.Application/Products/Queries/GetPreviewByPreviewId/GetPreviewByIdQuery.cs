using IMS.Application.Contracts.CsvFileReader;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Queries.GetPreviewByPreviewId
{
    public record GetPreviewByIdQuery(string PreviewId) : IRequest<Result<List<ProductCSVModel>>>;

}
