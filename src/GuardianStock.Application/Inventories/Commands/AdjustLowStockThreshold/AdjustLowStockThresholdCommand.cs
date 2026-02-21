using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Inventories.Commands.AdjustLowStockThreshold;

public record AdjustLowStockThresholdCommand(Guid InventoryId, int NewLowStockThreshold) : IRequest<Result>;
