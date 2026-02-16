using FluentValidation;
using IMS.Domain.Core.Errors;
using IMS.Domain.Inventories;

namespace IMS.Application.Inventories.Commands.AdjustLowStockThreshold;

public class AdjustLowStockThresholdCommandValidator : AbstractValidator<AdjustLowStockThresholdCommand>
{
    public AdjustLowStockThresholdCommandValidator()
    {
        RuleFor(x => x.InventoryId)
            .NotEmpty()
            .WithMessage(Errors.InventoryErrors.InventoryIdIsRequired.Description)
            .WithErrorCode(Errors.InventoryErrors.InventoryIdIsRequired.Code);

        RuleFor(x => x.NewLowStockThreshold)
            .GreaterThanOrEqualTo(Inventory.MinimumLowStockThreshold)
            .WithMessage(Errors.InventoryErrors.QuantityCannotBeLowerThanThreshold.Description)
            .WithErrorCode(Errors.InventoryErrors.QuantityCannotBeLowerThanThreshold.Code);

    }
}
