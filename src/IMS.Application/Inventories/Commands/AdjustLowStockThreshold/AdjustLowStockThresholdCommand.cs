using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Inventories.Commands.AdjustLowStockThreshold;

public record AdjustLowStockThresholdCommand(Guid InventoryId, int NewLowStockThreshold) : IRequest<Result>;
