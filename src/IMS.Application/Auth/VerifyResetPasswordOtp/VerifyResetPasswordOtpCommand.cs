using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.VerifyResetPasswordOtp
{
    public record VerifyResetPasswordOtpCommand : IRequest<Result<VerifyResetPasswordOtpResponse>>
    {
        public string EmailAddress { get; init; }

        public string Otp { get; init; }
    }
}
