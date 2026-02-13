using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace IMS.Application.Products.Commands.UploadProductCsv
{
    public record UploadProductCsvCommand(IFormFile File, string jobId) : IRequest<Result<UploadProductCsvResponse>>;
}
