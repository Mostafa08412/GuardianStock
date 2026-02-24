using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.ResetPassword
{
    public record ResetPasswordCommand : IRequest<Result>
    {
        public string EmailAddress { get; init; }

        public string ResetPasswordToken { get; init; }

        public string NewPassword { get; init; }
    }
}
