using FluentValidation;
using GuardianStock.Domain.Core.Errors;

namespace GuardianStock.Application.LowStockAlerts.Commands.SendLowStockEmail
{
    public class SendLowStockEmailValidator : AbstractValidator<SendLowStockEmailCommand>
    {
        public SendLowStockEmailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage(Errors.InventoryErrors.ProductIdIsRequired.Description)
                .WithErrorCode(Errors.InventoryErrors.ProductIdIsRequired.Code);
        }
    }

}
