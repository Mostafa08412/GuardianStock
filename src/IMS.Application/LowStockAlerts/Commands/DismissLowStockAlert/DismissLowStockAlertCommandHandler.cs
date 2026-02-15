using IMS.Domain.Abstractions;
using IMS.Domain.Core.Errors;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Commands.DismissLowStockAlert;

public class DismissLowStockAlertCommandHandler : IRequestHandler<DismissLowStockAlertCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DismissLowStockAlertCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DismissLowStockAlertCommand request, CancellationToken cancellationToken)
    {
        var inventory = await _unitOfWork.Inventories.GetByIdAsync(request.InventoryId, cancellationToken);

        if (inventory is null)
        {
            return Result.Failure(Errors.InventoryErrors.NotFound);
        }

        // Add Error handling if there is no active low stock alert to dismiss



        inventory.DismissLowStockAlert();

        return Result.Success();
    }
}
