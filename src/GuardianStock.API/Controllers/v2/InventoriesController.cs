using Asp.Versioning;
using GuardianStock.Application.Common.Models;
using GuardianStock.Application.Inventories.Commands.AdjustLowStockThreshold;
using GuardianStock.Application.Inventories.Queries.GetInventory;
using GuardianStock.Application.Inventories.Queries.GetStockSummary;
using GuardianStock.Application.Inventories.Queries.ListInventories;
using GuardianStock.Domain.Enums;
using GuardianStock.API.Contracts;
using GuardianStock.API.Contracts.Examples;
using GuardianStock.API.Infrastructure;
using GuardianStock.Application.Inventories.Queries.GetInventory;
using GuardianStock.Application.Inventories.Queries.GetStockSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace GuardianStock.API.Controllers.v2
{
    [ApiController]
    [ApiVersion(2.0)]
    [Route(ApiRoutes.Inventories.Base)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]

    public class InventoriesController : BaseController
    {
        public InventoriesController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Retrieves a paginated list of inventories with filtering and sorting support.
        /// Supported sorting columns: ProductName, ProductPrice, Stock (default: Id).
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <returns>A paginated list of inventory summaries.</returns>
        [Authorize]
        [HttpGet(ApiRoutes.Inventories.GetAll)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<InventoryListItemDto>>))]
        [SwaggerOperation(
            Summary = "List inventories",
            Description = "Fetches a paginated list of inventories with support for searching (product name, SKU) and " +
            "filtering by stock status (Healthy, Low, Critical) and price range." +
            "\nSupported sorting columns: ProductName, ProductPrice, Stock (default: Id).",
            OperationId = "ListInventories"
        )]
        public async Task<IActionResult> GetAll([FromQuery] ListInventoriesQuery query)
        {
            var result = await _sender.Send(query);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves detailed information for a specific inventory item.
        /// </summary>
        /// <param name="inventoryId">The unique identifier of the inventory item.</param>
        /// <returns>The inventory details.</returns>
        [HttpGet(ApiRoutes.Inventories.GetById)]
        [Authorize]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<InventoryDetailsDto>))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Get inventory by ID",
            Description = "Retrieves full details for a single inventory item using its unique identifier.",
            OperationId = "GetInventoryById"
        )]
        public async Task<IActionResult> GetById([FromRoute] Guid inventoryId)
        {
            var result = await _sender.Send(new GetInventoryQuery(inventoryId));

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves a summary of the overall stock status across all inventories.
        /// </summary>
        /// <returns>Counts of products in Normal, Low, and Critical stock states.</returns>
        [Authorize]
        [HttpGet(ApiRoutes.Inventories.GetSummary)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<StockSummaryDto>))]
        [SwaggerOperation(
            Summary = "Get stock summary",
            Description = "Provides a high-level overview of current inventory health levels.",
            OperationId = "GetStockSummary"
        )]
        public async Task<IActionResult> GetSummary()
        {
            var result = await _sender.Send(new GetStockSummaryQuery());

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Updates the low stock threshold for a specific inventory item.
        /// </summary>
        /// <param name="inventoryId">The unique identifier of the inventory item.</param>
        /// <param name="command">The threshold adjustment details.</param>
        /// <returns>No content on success.</returns>
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
        [HttpPost(ApiRoutes.Inventories.AdjustThreshold)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Adjust threshold",
            Description = "Changes the point at which an inventory item triggers a low stock alert.",
            OperationId = "AdjustInventoryThreshold"
        )]
        public async Task<IActionResult> AdjustThreshold([FromRoute] Guid inventoryId, [FromBody] AdjustLowStockThresholdCommand command)
        {
            if (inventoryId != command.InventoryId)
            {
                return BadRequest("Inventory Id mismatch.");
            }

            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }


    }
}
