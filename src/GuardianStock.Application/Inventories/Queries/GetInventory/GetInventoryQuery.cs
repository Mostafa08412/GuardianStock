using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Inventories.Queries.GetInventory;

public record GetInventoryQuery(Guid InventoryId) : IRequest<Result<InventoryDetailsDto>>;
