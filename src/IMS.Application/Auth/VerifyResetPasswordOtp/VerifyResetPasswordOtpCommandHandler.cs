using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.VerifyResetPasswordOtp
{
    public class VerifyResetPasswordOtpCommandHandler : IRequestHandler<VerifyResetPasswordOtpCommand, Result<VerifyResetPasswordOtpResponse>>
    {
        private readonly IIdentityService _identityService;

        public VerifyResetPasswordOtpCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<VerifyResetPasswordOtpResponse>> Handle(VerifyResetPasswordOtpCommand request, CancellationToken cancellationToken)
        {
            var result = await _identityService.VerifyResetPasswordOTP(request.EmailAddress, request.Otp, cancellationToken);

            if (result.IsFailure)
            {
                return Result<VerifyResetPasswordOtpResponse>.Failure(result.Errors);
            }

            var response = new VerifyResetPasswordOtpResponse
            {

                ResetPasswordToken = result.Value!
            };

            return Result<VerifyResetPasswordOtpResponse>.Success(response);
        }
    }
}
