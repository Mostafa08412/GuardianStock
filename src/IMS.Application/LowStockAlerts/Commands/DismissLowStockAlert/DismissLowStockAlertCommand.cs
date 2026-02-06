using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Commands.DismissLowStockAlert;

public record DismissLowStockAlertCommand(Guid ProductId) : IRequest<Result>;
