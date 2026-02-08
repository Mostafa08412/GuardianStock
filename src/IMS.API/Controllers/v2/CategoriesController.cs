using Asp.Versioning;
using IMS.API.Contracts;
using IMS.API.Contracts.Examples;
using IMS.API.Infrastructure;
using IMS.Application.Categories.Commands.CreateCategory;
using IMS.Application.Categories.Commands.DeleteCategory;
using IMS.Application.Categories.Commands.UpdateCategory;
using IMS.Application.Categories.Queries;
using IMS.Application.Categories.Queries.GetCategory;
using IMS.Application.Categories.Queries.ListCategories;
using IMS.Application.Common.Models;
using IMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace IMS.API.Controllers.v2
{
    [ApiController]
    [ApiVersion(2.0)]
    [Route(ApiRoutes.Categories.Base)]
    [Authorize(Roles = Roles.Admin)]
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
        /// <param name="searchTerm">The search term to filter categories.</param>
        /// <param name="sortBy">The field to sort by.</param>
        /// <param name="sortDescending">Whether to sort in descending order.</param>
        /// <param name="page">The page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list of categories.</returns>
        [HttpGet(ApiRoutes.Categories.GetAll)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<CategoryListItemDto>>))]
        [SwaggerOperation(
            Summary = "List categories",
            Description = "Retrieves a paginated list of categories with support for searching, sorting, and pagination." +
            "\nSupported sorting columns: name (default), productcount.\r\n",
            OperationId = "ListCategories"
        )]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? searchTerm,
            [FromQuery] string? sortBy,
            [FromQuery] bool sortDescending = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _sender.Send(new ListCategoriesQuery(searchTerm, sortBy, sortDescending, page, pageSize));

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="request">The category creation request.</param>
        /// <returns>The unique identifier of the created category.</returns>
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
        public async Task<IActionResult> Delete([FromRoute] Guid categoryId)
        {
            var result = await _sender.Send(new DeleteCategoryCommand(categoryId));

            return HandleResult(result, ApplicationStatusCodes.NoContent);
        }


    }
}
