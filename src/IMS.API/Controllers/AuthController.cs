using IMS.API.Contracts;
using IMS.API.Extensions;
using IMS.API.Infrastructure;
using IMS.Application.Auth.ChangePassword;
using IMS.Application.Auth.Common;
using IMS.Application.Auth.Logout;
using IMS.Application.Auth.RefreshToken;
using IMS.Application.Auth.SendEmail;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _sender.Send(request);

            return result.Map(
                onSuccess: success =>
                    Ok(new OkApiResponse<AuthenticationResponse>(success.Value!)),
                onFailure: failure =>
                    MapResultFailureToActionResult(failure)
            );
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _sender.Send(request);

            return result.Map(
                onSuccess: _ =>
                    NoContent(),
                onFailure: failure =>
                    MapResultFailureToActionResult(failure)
            );
        }

        [Authorize]
        [HttpPost(ApiRoutes.Authentication.ChangePassword)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
        {
            var result = await _sender.Send(request);

            return result.Map(
                onSuccess: _ =>
                    NoContent(),
                onFailure: failure =>
                    MapResultFailureToActionResult(failure)
            );
        }

        [Authorize]
        [HttpPost(ApiRoutes.Authentication.Logout)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand request)
        {
            var result = await _sender.Send(request);

            return result.Map(
                onSuccess: _ =>
                    NoContent(),
                onFailure: failure =>
                    MapResultFailureToActionResult(failure)
            );
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.RefreshToken)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            var result = await _sender.Send(request);

            return result.Map(
                onSuccess: success =>
                    Ok(new OkApiResponse<AuthenticationResponse>(success.Value!)),
                onFailure: failure =>
                    MapResultFailureToActionResult(failure)
            );
        }


        [AllowAnonymous]
        [HttpPost("send-email")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand request)
        {
            var result = await _sender.Send(request);

            return result.Map(
                onSuccess: success =>
                   NoContent(),
                onFailure: failure =>
                    MapResultFailureToActionResult(failure)
            );
        }
    }
}
