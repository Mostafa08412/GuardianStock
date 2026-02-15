using Asp.Versioning;
using IMS.API.Contracts;
using IMS.API.Contracts.Examples;
using IMS.API.Infrastructure;
using IMS.Application.Dashboard.Queries;
using IMS.Application.Dashboard.Queries.GetDashboardStats;
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
    [Route(ApiRoutes.Dashboard.Base)]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager},{Roles.Staff}")]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]

    public class DashboardController : BaseController
    {
        public DashboardController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Retrieves statistical data and chart information for the application dashboard.
        /// </summary>
        /// <returns>Comprehensive dashboard statistics including sales, categories, and inventory alerts.</returns>
        [HttpGet(ApiRoutes.Dashboard.GetStats)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<DashboardDto>))]
        [SwaggerOperation(
            Summary = "Get dashboard statistics",
            Description = "Fetches a consolidated set of metrics and data points for the dashboard UI.",
            OperationId = "GetDashboardStats"
        )]
        public async Task<IActionResult> GetStats()
        {
            var result = await _sender.Send(new GetDashboardStatsQuery());

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }
    }
}
