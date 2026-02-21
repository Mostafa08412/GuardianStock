using Asp.Versioning;
using GuardianStock.Application.Auth.ChangePassword;
using GuardianStock.Application.Auth.Common;
using GuardianStock.Application.Auth.ForgetPassword;
using GuardianStock.Application.Auth.GoogleLogin;
using GuardianStock.Application.Auth.Logout;
using GuardianStock.Application.Auth.RefreshToken;
using GuardianStock.Application.Auth.ResetPassword;
using GuardianStock.Application.Auth.UpdateProfile;
using GuardianStock.Application.Auth.VerifyResetPasswordOtp;
using GuardianStock.Application.Contracts.Identity;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.API.Contracts;
using GuardianStock.API.Contracts.Examples;
using GuardianStock.API.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using LoginRequest = GuardianStock.Application.Auth.Login.LoginRequest;
using RegisterRequest = GuardianStock.Application.Auth.Register.RegisterCommand;

namespace GuardianStock.API.Controllers.v1
{
    [ApiController]
    [ApiVersion(1.0)]
    [Route(ApiRoutes.Authentication.Base)]
    [SwaggerResponse((int)ApplicationStatusCodes.Unauthorized, "The request is missing a valid authentication token.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.Forbidden, "The authenticated user does not have the necessary permissions for this resource.", typeof(ApiResponse))]
    [SwaggerResponse((int)ApplicationStatusCodes.BadRequest, "The request payload is invalid or malformed.", typeof(ApiResponse))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Unauthorized, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.Forbidden, typeof(ApiResponseExample))]
    [SwaggerResponseExample((int)ApplicationStatusCodes.BadRequest, typeof(ApiResponseExample))]
    public class AuthController : BaseController
    {
        public AuthController(ISender sender)
            : base(sender)
        {
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="request">The login credentials (email and password).</param>
        /// <returns>An authentication response containing the JWT token and user details.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.Login)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<AuthenticationResponse>))]
        [ProducesResponseType((int)ApplicationStatusCodes.Unauthorized, Type = typeof(ApiResponse<AuthenticationResponse>))]
        [SwaggerOperation(
            Summary = "Login user",
            Description = "Authenticates a user with email and password, returning a JWT token for access.",
            OperationId = "Login"
        )]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Result<AuthenticationResponse> result = await _sender.Send(request);
            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="request">The user registration details.</param>
        /// <returns>A response indicating the result of the registration process.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.Register)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<AuthenticationResponse>))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Register user",
            Description = "Creates a new user account with the provided details.",
            OperationId = "Register"
        )]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);

        }

        /// <summary>
        /// Changes the password for the currently authenticated user.
        /// </summary>
        /// <param name="request">The change password request containing old and new passwords.</param>
        /// <returns>No content if successful.</returns>
        [Authorize]
        [HttpPost(ApiRoutes.Authentication.ChangePassword)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Change password",
            Description = "Updates the password for the currently logged-in user.",
            OperationId = "ChangePassword"
        )]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Logs out the current user by invalidating their refresh token.
        /// </summary>
        /// <param name="request">The logout request containing the refresh token to invalidate.</param>
        /// <returns>No content if successful.</returns>
        [Authorize]
        [HttpPost(ApiRoutes.Authentication.Logout)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Logout user",
            Description = "Invalidates the user's refresh token, effectively logging them out.",
            OperationId = "Logout"
        )]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Refreshes the JWT token using a valid refresh token.
        /// </summary>
        /// <param name="request">The refresh token request.</param>
        /// <returns>A new authentication response with a fresh JWT token.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.RefreshToken)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<AuthenticationResponse>))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Refresh token",
            Description = "Obtains a new JWT token using a valid refresh token.",
            OperationId = "RefreshToken"
        )]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
        {
            var result = await _sender.Send(request);


            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Authenticates a user using a Google ID token.
        /// </summary>
        /// <param name="request">The Google login request containing the ID token.</param>
        /// <returns>An authentication response containing the JWT token and user details.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.GoogleLogin)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<AuthenticationResponse>))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Google login",
            Description = "Authenticates a user using a Google ID token. Creates a new account if the user does not exist.",
            OperationId = "GoogleLogin"
        )]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Updates the profile information for the currently authenticated user.
        /// </summary>
        /// <param name="request">The update profile request containing the new first name and last name.</param>
        /// <returns>The updated user details.</returns>
        [Authorize]
        [HttpPut(ApiRoutes.Authentication.UpdateProfile)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<IdentityUserDto>))]
        [ProducesResponseType((int)ApplicationStatusCodes.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Update profile",
            Description = "Updates the first name and last name for the currently logged-in user.",
            OperationId = "UpdateProfile"
        )]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }


        /// <summary>
        /// Sends a password reset OTP to the user's email address.
        /// </summary>
        /// <param name="request">The forget password request containing the email address.</param>
        /// <returns>A response indicating the OTP was sent successfully.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.ForgetPassword)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Forget password",
            Description = "Generates a one-time password and sends it to the user's email for password reset.",
            OperationId = "ForgetPassword"
        )]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Verifies the password reset OTP and returns a reset password token.
        /// </summary>
        /// <param name="request">The verify OTP request containing the email and OTP.</param>
        /// <returns>A reset password token to be used for resetting the password.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.VerifyResetPasswordOtp)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse<VerifyResetPasswordOtpResponse>))]
        [SwaggerOperation(
            Summary = "Verify reset password OTP",
            Description = "Validates the OTP sent to the user's email and returns a token for password reset.",
            OperationId = "VerifyResetPasswordOtp"
        )]
        public async Task<IActionResult> VerifyResetPasswordOtp([FromBody] VerifyResetPasswordOtpCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }

        /// <summary>
        /// Resets the user's password using the reset password token.
        /// </summary>
        /// <param name="request">The reset password request containing the token and new password.</param>
        /// <returns>A response indicating the password was reset successfully.</returns>
        [AllowAnonymous]
        [HttpPost(ApiRoutes.Authentication.ResetPassword)]
        [ProducesResponseType((int)ApplicationStatusCodes.Ok, Type = typeof(ApiResponse))]
        [SwaggerOperation(
            Summary = "Reset password",
            Description = "Resets the user's password using the reset password token obtained from OTP verification.",
            OperationId = "ResetPassword"
        )]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand request)
        {
            var result = await _sender.Send(request);

            return HandleResult(result, ApplicationStatusCodes.Ok);
        }
    }
}
