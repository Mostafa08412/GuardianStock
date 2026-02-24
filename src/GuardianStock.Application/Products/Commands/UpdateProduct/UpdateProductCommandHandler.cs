using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileManagerService _fileManagerService;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, IFileManagerService fileManagerService)
    {

        _unitOfWork = unitOfWork;
        _fileManagerService = fileManagerService;
    }

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return Result.Failure(Errors.ProductErrors.ProductNotFound);
        }

        if (request.Name is not null && !string.IsNullOrWhiteSpace(request.Name))
        {
            var renameResult = product.Rename(request.Name);
            if (renameResult.IsFailure) return renameResult;
        }

        if (request.Description is not null && !string.IsNullOrWhiteSpace(request.Description))
        {
            var redescribeResult = product.Redescription(request.Description);
            if (redescribeResult.IsFailure) return redescribeResult;
        }

        if (request.CategoryId.HasValue)
        {
            var recategorizeResult = product.Recategorize(request.CategoryId.Value);
            if (recategorizeResult.IsFailure) return recategorizeResult;
        }

        if (request.Price.HasValue)
        {
            var updatePriceResult = product.UpdatePrice(request.Price.Value);
            if (updatePriceResult.IsFailure) return updatePriceResult;
        }

        if (request.Supplier is not null && !string.IsNullOrWhiteSpace(request.Supplier))
        {
            var updateSupplierResult = product.UpdateSupplier(request.Supplier);
            if (updateSupplierResult.IsFailure) return updateSupplierResult;
        }

        if (request.Image is not null)
        {
            // 1. Delete old image if exists
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var imagefilename = product.ImageUrl.Split("images/")[1];
                var oldFileName = Path.GetFileNameWithoutExtension(imagefilename);
                var oldExtension = Path.GetExtension(imagefilename).TrimStart('.');
                _fileManagerService.DeleteFileIfExists(oldFileName, "products-images", oldExtension);
            }

            // 2. Upload new image
            var fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(request.Image.FileName).TrimStart('.');
            await _fileManagerService.UploadFileAsync(request.Image, "products-images", fileName, cancellationToken);
            var imageUrl = $"api/v2/products/images/{fileName}.{extension}";

            product.UpdateImage(imageUrl);
        }

        if (request.LowStockAlertThreshold.HasValue)
        {
            var inventory = await _unitOfWork.Inventories.GetByProductIdAsync(request.ProductId, cancellationToken);

            if (inventory is not null)
            {
                var result = inventory.AdjustLowStockThreshold(request.LowStockAlertThreshold.Value!);
                if (!result.IsSuccess)
                    return Result.Failure(result.Error);

            }
        }

        return Result.Success();
    }
}
