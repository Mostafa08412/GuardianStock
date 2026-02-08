using IMS.API.Contracts;
using IMS.API.Infrastructure;
using IMS.Application.Common.Models;
using IMS.Application.Users.Commands.ActivateUser;
using IMS.Application.Users.Commands.DeactivateUser;
using IMS.Application.Users.Commands.UpdateUser;
using IMS.Application.Users.Queries;
using IMS.Application.Users.Queries.GetUser;
using IMS.Application.Users.Queries.ListUsers;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMS.Api.Controllers;

[ApiController]
[Route("api/v1/" + ApiRoutes.Users.Base)]
[AllowAnonymous]
public class UsersController : BaseController
{
    public UsersController(ISender sender)
        : base(sender)
    {
    }

    [HttpGet]
    [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<PaginatedList<UserListItemDto>>))]
    public async Task<IActionResult> ListUsers([FromQuery] ListUsersQuery query)
    {

        var paginatedList = await _sender.Send(query);

        var result = Result<PaginatedList<UserListItemDto>>.Success(paginatedList);

        return HandleResult(result, ApplicationStatusCodes.Ok);
    }

    [HttpGet(ApiRoutes.Users.GetById)]
    [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<UserDto>))]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound, Type = typeof(ApiResponse<UserDto>))]
    public async Task<IActionResult> GetUser([FromRoute] GetUserQuery query)
    {
        var result = await _sender.Send(query);

        return HandleResult(result, ApplicationStatusCodes.Ok);
    }

    [HttpPut(ApiRoutes.Users.Update)]
    [ProducesResponseType((int)ApplicationStatusCodes.NoContent)]
    [ProducesResponseType((int)ApplicationStatusCodes.BadRequest)]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound)]
    public async Task<IActionResult> UpdateUser(string userId, [FromBody] UpdateUserCommand command)
    {
        if (userId != command.UserId)
        {
            return BadRequest("User ID mismatch");
        }

        var result = await _sender.Send(command);

        return HandleResult(result, ApplicationStatusCodes.NoContent);
    }

    [HttpPost(ApiRoutes.Users.Activate)]
    [ProducesResponseType((int)ApplicationStatusCodes.NoContent)]
    [ProducesResponseType((int)ApplicationStatusCodes.BadRequest)]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound)]
    public async Task<IActionResult> ActivateUser(string userId)
    {
        var command = new ActivateUserCommand(userId);
        var result = await _sender.Send(command);

        return HandleResult(result, ApplicationStatusCodes.NoContent);
    }

    [HttpPost(ApiRoutes.Users.Deactivate)]
    [ProducesResponseType((int)ApplicationStatusCodes.NoContent)]
    [ProducesResponseType((int)ApplicationStatusCodes.BadRequest)]
    [ProducesResponseType((int)ApplicationStatusCodes.NotFound)]
    public async Task<IActionResult> DeactivateUser(string userId)
    {
        var command = new DeactivateUserCommand(userId);
        var result = await _sender.Send(command);

        return HandleResult(result, ApplicationStatusCodes.NoContent);
    }
}
