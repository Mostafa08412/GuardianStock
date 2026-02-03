using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Commands.AdjustLowStockThreshold;

public record AdjustLowStockThresholdCommand(Guid ProductId, int NewLowStockThreshold) : IRequest<Result>;
