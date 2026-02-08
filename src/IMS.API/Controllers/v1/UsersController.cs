using Asp.Versioning;
using IMS.API.Contracts;
using IMS.API.Contracts.Examples;
using IMS.API.Infrastructure;
using IMS.Application.Common.Models;
using IMS.Application.Users.Commands.ActivateUser;
using IMS.Application.Users.Commands.DeactivateUser;
using IMS.Application.Users.Commands.UpdateUser;
using IMS.Application.Users.Queries;
using IMS.Application.Users.Queries.GetUser;
using IMS.Application.Users.Queries.ListUsers;
using IMS.Domain.Core.Primitives.Result;
using IMS.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace IMS.API.Controllers.v1;

[ApiController]
[Route(ApiRoutes.Users.Base)]
[Authorize(Roles = Roles.Admin)]
[SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
[SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the 'Admin' role required for this resource.", typeof(ApiResponse))]
[SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
[SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
[SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
[SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]
[ApiVersion(1)]

public class UsersController : BaseController
{
    public UsersController(ISender sender)
        : base(sender)
    {
    }

    /// <summary>
    /// Retrieves a paginated list of all users.
    /// </summary>
    /// <param name="query">Pagination and filtering parameters.</param>
    /// <returns>A paginated list of user summary objects.</returns>
    [HttpGet]
    [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<UserListItemDto>>))]
    [SwaggerOperation(
        Summary = "Returns all users",
        Description = "Fetches a paginated list of users. Requires Admin privileges.",
        OperationId = "GetAllUsers"
    )]
    public async Task<IActionResult> ListUsers([FromQuery] ListUsersQuery query)
    {
        var paginatedList = await _sender.Send(query);
        var result = Result<PaginatedList<UserListItemDto>>.Success(paginatedList);
        return HandleResult(result, ApplicationStatusCodes.Ok);
    }

    /// <summary>
    /// Retrieves detailed information for a specific user.
    /// </summary>
    /// <param name="query">The query containing the User ID.</param>
    /// <returns>Detailed user profile information.</returns>
    [HttpGet(ApiRoutes.Users.GetById)]
    [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<UserDto>))]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse<UserDto>))]
    [SwaggerOperation(
        Summary = "Get user by ID",
        Description = "Retrieves full details for a single user using their unique identifier.",
        OperationId = "GetUserById"
    )]
    public async Task<IActionResult> GetUser([FromRoute] GetUserQuery query)
    {
        var result = await _sender.Send(query);
        return HandleResult(result, ApplicationStatusCodes.Ok);
    }

    /// <summary>
    /// Updates an existing user's profile information.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="command">The updated user data.</param>
    [HttpPut(ApiRoutes.Users.Update)]
    [ProducesResponseType((int)ApplicationStatusCodes.NoContent)]
    [ProducesResponseType((int)ApplicationStatusCodes.BadRequest)]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound)]
    [SwaggerOperation(
        Summary = "Update user",
        Description = "Updates user profile details. The ID in the route must match the ID in the request body.",
        OperationId = "UpdateUser"
    )]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserCommand command)
    {
        if (userId != command.UserId)
        {
            return BadRequest("User ID mismatch");
        }

        var result = await _sender.Send(command);
        return HandleResult(result, ApplicationStatusCodes.NoContent);
    }

    /// <summary>
    /// Re-activates a deactivated user account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpPost(ApiRoutes.Users.Activate)]
    [ProducesResponseType((int)ApplicationStatusCodes.NoContent)]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound)]
    [SwaggerOperation(
        Summary = "Activate user",
        Description = "Restores access for a previously deactivated user account.",
        OperationId = "ActivateUser"
    )]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        var command = new ActivateUserCommand(userId);
        var result = await _sender.Send(command);
        return HandleResult(result, ApplicationStatusCodes.NoContent);
    }

    /// <summary>
    /// Deactivates a user account to prevent login.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    [HttpPost(ApiRoutes.Users.Deactivate)]
    [ProducesResponseType((int)ApplicationStatusCodes.NoContent)]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound)]
    [SwaggerOperation(
        Summary = "Deactivate user",
        Description = "Suspends a user's account. The user will no longer be able to log in until re-activated.",
        OperationId = "DeactivateUser"
    )]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var command = new DeactivateUserCommand(userId);
        var result = await _sender.Send(command);
        return HandleResult(result, ApplicationStatusCodes.NoContent);
    }
}