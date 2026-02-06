using FluentValidation;

namespace IMS.Application.LowStockAlerts.Commands.SendLowStockEmail
{
    public class SendLowStockEmailValidator : AbstractValidator<SendLowStockEmailCommand>
    {
        public SendLowStockEmailValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty()
                .WithMessage("ProductId is required to send low stock alert.");
        }
    }

}
