using IMS.API.Contracts;
using IMS.API.Infrastructure;
using IMS.Application.Auth.ChangePassword;
using IMS.Application.Auth.Common;
using IMS.Application.Auth.Logout;
using IMS.Application.Auth.RefreshToken;
using IMS.Domain.Core.Primitives.Result;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoginRequest = IMS.Application.Auth.Login.LoginRequest;
using RegisterRequest = IMS.Application.Auth.Register.RegisterCommand;

namespace IMS.API.Controllers
{
    [ApiController]
    [Route(ApiRoutes.Authentication.Base)]
    public class AuthController : BaseController
    {
        public AuthController(ISender sender)
            : base(sender)
        {
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.Login)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<AuthenticationResponse>))]
        [ProducesResponseType((int)ApplicationStatusCodes.Unauthorized, Type = typeof(ApiResponse<AuthenticationResponse>))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Result<AuthenticationResponse> result = await _sender.Send(request);
            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);

        }

        [Authorize]
        [HttpPost(ApiRoutes.Authentication.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        [Authorize]
        [HttpPost(ApiRoutes.Authentication.Logout)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.RefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            var result = await _sender.Send(request);


            return HandleResult(result, ApplicationStatusCodes.Ok);
        }
    }
}
