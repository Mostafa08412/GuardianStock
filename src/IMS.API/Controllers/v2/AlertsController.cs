using Asp.Versioning;
using IMS.API.Contracts;
using IMS.API.Contracts.Examples;
using IMS.API.Infrastructure;
using IMS.Application.Common.Models;
using IMS.Application.LowStockAlerts.Commands.AdjustLowStockThreshold;
using IMS.Application.LowStockAlerts.Commands.DismissLowStockAlert;
using IMS.Application.LowStockAlerts.Queries.GetLowStockAlerts;
using IMS.Application.LowStockAlerts.Queries.GetStockAlertDetails;
using IMS.Application.LowStockAlerts.Queries.GetStockSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using Roles = IMS.Domain.Enums.Roles;

namespace IMS.API.Controllers.v2
{
    [ApiController]
    [ApiVersion(2.0)]
    [Route(ApiRoutes.Alerts.Base)]
    [Authorize(Roles = Roles.Admin)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]

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
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<LowStockAlertDto>>))]
        [SwaggerOperation(
            Summary = "List stock alerts",
            Description = "Fetches a paginated list of products that have fallen below their low stock threshold.",
            OperationId = "ListAlerts"
        )]
        public async Task<IActionResult> ListAlerts([FromQuery] GetLowStockAlertsQuery query)
        {
            var result = await _sender.Send(query);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves detailed information for a specific stock alert.
        /// </summary>
        /// <param name="inventoryId">The unique identifier of the inventory item.</param>
        /// <returns>Details about the stock alert.</returns>
        [HttpGet(ApiRoutes.Alerts.GetDetails)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<StockAlertDetailsDto>))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Get alert details",
            Description = "Retrieves comprehensive information about a specific stock alert, including product history.",
            OperationId = "GetAlertDetails"
        )]
        public async Task<IActionResult> GetDetails([FromRoute] Guid inventoryId)
        {
            var result = await _sender.Send(new GetStockAlertDetailsQuery(inventoryId));

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves a summary of the overall stock status across all products.
        /// </summary>
        /// <returns>Counts of products in Normal, Low, and Critical stock states.</returns>
        [HttpGet(ApiRoutes.Alerts.GetSummary)]
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
        /// Updates the low stock threshold for a specific product.
        /// </summary>
        /// <param name="command">The threshold adjustment details.</param>
        /// <returns>No content on success.</returns>
        [HttpPost(ApiRoutes.Alerts.AdjustThreshold)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Adjust threshold",
            Description = "Changes the point at which a product triggers a low stock alert.",
            OperationId = "AdjustThreshold"
        )]
        public async Task<IActionResult> AdjustThreshold([FromBody] AdjustLowStockThresholdCommand command)
        {
            var result = await _sender.Send(command);

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }

        /// <summary>
        /// Dismisses an active low stock alert for a specific product.
        /// </summary>
        /// <param name="productId">The unique identifier of the product.</param>
        /// <returns>No content on success.</returns>
        [HttpPost(ApiRoutes.Alerts.Dismiss)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Dismiss alert",
            Description = "Acknowledges and dismisses the current low stock alert for a product.",
            OperationId = "DismissAlert"
        )]
        public async Task<IActionResult> DismissAlert([FromRoute] Guid productId)
        {
            var result = await _sender.Send(new DismissLowStockAlertCommand(productId));

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }
    }
}
