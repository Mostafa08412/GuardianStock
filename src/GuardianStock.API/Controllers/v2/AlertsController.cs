using Asp.Versioning;
using GuardianStock.Application.Common.Models;
using GuardianStock.Application.LowStockAlerts.Queries.ListLowStockAlerts;
using GuardianStock.API.Contracts;
using GuardianStock.API.Contracts.Examples;
using GuardianStock.API.Infrastructure;
using GuardianStock.Application.LowStockAlerts.Commands.DismissLowStockAlert;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Roles = GuardianStock.Domain.Enums.Roles;

namespace GuardianStock.API.Controllers.v2
{
    [ApiController]
    [ApiVersion(2.0)]
    [Route(ApiRoutes.Alerts.Base)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]

    public class AlertsController : BaseController
    {
        public AlertsController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Retrieves a paginated list of low stock alerts.
        /// </summary>
        /// <param name="query">Filtering and pagination parameters.</param>
        /// <returns>A paginated list of stock alerts.</returns>
        [HttpGet(ApiRoutes.Alerts.GetAll)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<LowStockAlertListItemDto>>))]
        [SwaggerOperation(
            Summary = "List stock alerts",
            Description = "Fetches a paginated list of products that have fallen below their low stock threshold.",
            OperationId = "ListAlerts"
        )]
        public async Task<IActionResult> ListAlerts([FromQuery] ListLowStockAlertsQuery query)
        {
            var result = await _sender.Send(query);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Dismisses an active low stock alert for a specific product.
        /// </summary>
        /// <param name="inventoryId">The unique identifier of the product inventory.</param>
        /// <returns>No content on success.</returns>
        [HttpPost(ApiRoutes.Alerts.Dismiss)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Dismiss alert",
            Description = "Acknowledges and dismisses the current low stock alert for a product.",
            OperationId = "DismissAlert"
        )]
        public async Task<IActionResult> DismissAlert([FromRoute] Guid inventoryId)
        {
            var result = await _sender.Send(new DismissLowStockAlertCommand(inventoryId));

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }
    }
}
