using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Commands.AdjustLowStockThreshold;

public record AdjustLowStockThresholdCommand(Guid InventoryId, int NewLowStockThreshold) : IRequest<Result>;
