using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
    {

        _unitOfWork = unitOfWork;
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
