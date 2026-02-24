using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.VerifyResetPasswordOtp
{
    public record VerifyResetPasswordOtpCommand : IRequest<Result<VerifyResetPasswordOtpResponse>>
    {
        public string EmailAddress { get; init; }

        public string Otp { get; init; }
    }
}
