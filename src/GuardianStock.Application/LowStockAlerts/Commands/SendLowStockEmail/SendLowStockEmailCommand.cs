using MediatR;

namespace GuardianStock.Application.LowStockAlerts.Commands.SendLowStockEmail
{

    //NOTE: This command is triggered by a background job.
    public record SendLowStockEmailCommand : IRequest
    {
        public Guid ProductId { get; init; }
        public Guid InventoryId { get; init; }
    }

}
