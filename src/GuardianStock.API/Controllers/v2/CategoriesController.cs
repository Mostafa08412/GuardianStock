using Asp.Versioning;
using GuardianStock.Application.Categories.Commands.CreateCategory;
using GuardianStock.Application.Categories.Commands.UpdateCategory;
using GuardianStock.Application.Categories.Queries;
using GuardianStock.Application.Categories.Queries.ListCategories;
using GuardianStock.Application.Common.Models;
using GuardianStock.Domain.Enums;
using GuardianStock.API.Contracts;
using GuardianStock.API.Contracts.Examples;
using GuardianStock.API.Infrastructure;
using GuardianStock.Application.Categories.Commands.DeleteCategory;
using GuardianStock.Application.Categories.Queries.GetCategory;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace GuardianStock.API.Controllers.v2
{
    [ApiController]
    [ApiVersion(2.0)]
    [Route(ApiRoutes.Categories.Base)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]

    public class CategoriesController : BaseController
    {
        public CategoriesController(ISender sender) : base(sender)
        {
        }

        /// <summary>
        /// Retrieves detailed information about a specific category.
        /// </summary>
        /// <param name="categoryId">The unique identifier of the category.</param>
        /// <returns>The category details.</returns>
        [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]

        [HttpGet(ApiRoutes.Categories.GetById)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<CategoryDetails>))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Get category by ID",
            Description = "Retrieves detailed information about a specific category using its unique identifier.",
            OperationId = "GetCategoryById"
        )]
        public async Task<IActionResult> GetById([FromRoute] Guid categoryId)
        {
            var result = await _sender.Send(new GetCategoryQuery(categoryId));

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Retrieves a paginated list of categories based on search and sort criteria.
        /// Supported sorting columns: name (default), productcount.
        /// </summary>
        /// <param name="query">Filtering, sorting, and pagination parameters.</param>
        /// <returns>A paginated list of categories.</returns>
        [Authorize]

        [HttpGet(ApiRoutes.Categories.GetAll)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<CategoryListItemDto>>))]
        [SwaggerOperation(
            Summary = "List categories",
            Description = "Retrieves a paginated list of categories with support for searching, sorting, and pagination." +
            "\nSupported sorting columns: name (default), productcount.\r\n",
            OperationId = "ListCategories"
        )]
        public async Task<IActionResult> GetAll(
            [FromQuery] ListCategoriesQuery query
)
        {
            var result = await _sender.Send(query);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="request">The category creation request.</param>
        /// <returns>The unique identifier of the created category.</returns>
        [Authorize(Roles = Roles.Admin)]

        [HttpPost(ApiRoutes.Categories.Create)]
        [ProducesResponseType((int)ApplicationStatusCodes.Created, Type = typeof(ApiResponse<Guid>))]
        [ProducesResponseType((int)ApplicationStatusCodes.Conflict, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Create category",
            Description = "Creates a new category with the provided details.",
            OperationId = "CreateCategory"
        )]
        public async Task<IActionResult> Create([FromBody] CreateCategoryCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Created);
        }

        /// <summary>
        /// Updates an existing category.
        /// </summary>
        /// <param name="categoryId">The unique identifier of the category to update.</param>
        /// <param name="request">The category update request.</param>
        /// <returns>No content on success.</returns>
        [Authorize(Roles = Roles.Admin)]

        [HttpPut(ApiRoutes.Categories.Update)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Update category",
            Description = "Updates the details of an existing category using its unique identifier. The ID in the route must match the ID in the request body.",
            OperationId = "UpdateCategory"
        )]
        public async Task<IActionResult> Update([FromRoute] Guid categoryId, [FromBody] UpdateCategoryCommand request)
        {
            if (categoryId != request.Id)
            {
                return BadRequest(("The provided category ID does not match the request body."));
            }

            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }

        /// <summary>
        /// Deletes a specific category.
        /// </summary>
        /// <param name="categoryId">The unique identifier of the category to delete.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete(ApiRoutes.Categories.Delete)]
        [ProducesResponseType((int)ApplicationStatusCodes.NoContent, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Delete category",
            Description = "Permanently deletes a category from the system.",
            OperationId = "DeleteCategory"
        )]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete([FromRoute] Guid categoryId)
        {
            var result = await _sender.Send(new DeleteCategoryCommand(categoryId));

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }


    }
}
