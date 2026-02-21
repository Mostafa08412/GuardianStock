using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.Inventories.Commands.AdjustLowStockThreshold;

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
