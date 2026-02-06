using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.LowStockAlerts.Queries.GetStockAlertDetails;

public record GetStockAlertDetailsQuery(Guid InventoryId) : IRequest<Result<StockAlertDetailsDto>>;
