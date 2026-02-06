using IMS.Application.Common.Errors;
using IMS.Application.Contracts.CsvFileReader;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace IMS.Application.Products.Queries.GetPreviewByPreviewId
{
    public class GetPreviewByIdHandler(HybridCache cache, ILogger<GetPreviewByIdHandler> logger)
    : IRequestHandler<GetPreviewByIdQuery, Result<List<ProductCSVModel>>>
    {
        public async Task<Result<List<ProductCSVModel>>> Handle(GetPreviewByIdQuery request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Attempting to retrieve upload preview for ID: {PreviewId}", request.PreviewId);


            var cachedData = await cache.GetOrCreateAsync<List<ProductCSVModel>>(
                key: $"preview_{request.PreviewId}",
                factory: _ => ValueTask.FromResult<List<ProductCSVModel>>(null!),
                cancellationToken: cancellationToken
            );

            if (cachedData is null)
            {
                logger.LogWarning("No preview data found in cache for ID: {PreviewId}", request.PreviewId);
                return Result<List<ProductCSVModel>>.Failure(ApplicationErrors.CsvReader.PreviewExpired);
            }

            logger.LogInformation("Successfully retrieved {Count} rows from cache for ID: {PreviewId}",
                cachedData.Count, request.PreviewId);

            return Result<List<ProductCSVModel>>.Success(cachedData);
        }
    }

}
