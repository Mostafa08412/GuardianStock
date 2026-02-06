using FluentValidation;

namespace IMS.Application.LowStockAlerts.Queries.GetStockAlertDetails;

public class GetStockAlertDetailsQueryValidator : AbstractValidator<GetStockAlertDetailsQuery>
{
    public GetStockAlertDetailsQueryValidator()
    {
        RuleFor(x => x.InventoryId)
            .NotEmpty().WithMessage("Inventory ID is required.");
    }
}
