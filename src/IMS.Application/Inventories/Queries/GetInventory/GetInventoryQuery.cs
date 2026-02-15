using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Inventories.Queries.GetInventory;

public record GetInventoryQuery(Guid InventoryId) : IRequest<Result<InventoryDetailsDto>>;
