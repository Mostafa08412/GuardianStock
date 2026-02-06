using FluentValidation;
using IMS.Domain.Inventories;

namespace IMS.Application.LowStockAlerts.Commands.AdjustLowStockThreshold;

public class AdjustLowStockThresholdCommandValidator : AbstractValidator<AdjustLowStockThresholdCommand>
{
    public AdjustLowStockThresholdCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();

        RuleFor(x => x.NewLowStockThreshold)
            .GreaterThanOrEqualTo(Inventory.MinimumLowStockThreshold)
            .WithMessage($"Threshold must be at least {Inventory.MinimumLowStockThreshold}.");
    }
}
