using FluentValidation;
using IMS.Domain.Core.Errors;
using IMS.Domain.Inventories;

namespace IMS.Application.Inventories.Commands.AdjustLowStockThreshold;

public class AdjustLowStockThresholdCommandValidator : AbstractValidator<AdjustLowStockThresholdCommand>
{
    public AdjustLowStockThresholdCommandValidator()
    {
        RuleFor(x => x.InventoryId).NotEmpty();

        RuleFor(x => x.NewLowStockThreshold)
            .GreaterThanOrEqualTo(Inventory.MinimumLowStockThreshold)
            .WithMessage(Errors.InventoryErrors.QuantityCannotBeLowerThanThreshold.Description);
    }
}
