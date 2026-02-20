using GuardianStock.Domain.Abstractions;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Domain.Core.Errors;
using MediatR;

namespace GuardianStock.Application.LowStockAlerts.Commands.DismissLowStockAlert;

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
