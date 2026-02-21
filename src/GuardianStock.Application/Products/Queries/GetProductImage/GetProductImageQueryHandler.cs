using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Products.Queries.GetProductImage;

public class GetProductImageQueryHandler : IRequestHandler<GetProductImageQuery, Result<ProductImageDto>>
{
    private readonly IFileManagerService _fileManagerService;

    public GetProductImageQueryHandler(IFileManagerService fileManagerService)
    {
        _fileManagerService = fileManagerService;
    }

    public Task<Result<ProductImageDto>> Handle(GetProductImageQuery request, CancellationToken cancellationToken)
    {
        var filePath = _fileManagerService.GetFilePath(request.ImageName, "products-images");

        if (filePath is null)
            return Task.FromResult(Result<ProductImageDto>.Failure(Errors.ProductErrors.ImageNotFound));

        var extension = Path.GetExtension(filePath).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        return Task.FromResult(Result<ProductImageDto>.Success(new ProductImageDto(filePath, contentType)));
    }
}
