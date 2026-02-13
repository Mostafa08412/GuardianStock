using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Commands.AdjustLowStockThreshold;

public class AdjustLowStockThresholdCommandHandler : IRequestHandler<AdjustLowStockThresholdCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdjustLowStockThresholdCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AdjustLowStockThresholdCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _unitOfWork.Inventories.GetByIdAsync(request.InventoryId, cancellationToken);

        if (inventory is null)
        {
            return Result.Failure(Errors.InventoryErrors.NotFound);
        }

        var result = inventory.AdjustLowStockThreshold(request.NewLowStockThreshold);

        if (result.IsFailure)
        {
            return result;
        }

        return Result.Success();
    }
}
