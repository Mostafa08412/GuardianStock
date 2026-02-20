using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.LowStockAlerts.Commands.DismissLowStockAlert;

public record DismissLowStockAlertCommand(Guid InventoryId) : IRequest<Result>;
