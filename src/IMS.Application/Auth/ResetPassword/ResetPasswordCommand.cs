using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.ResetPassword
{
    public record ResetPasswordCommand : IRequest<Result>
    {
        public string EmailAddress { get; init; }

        public string ResetPasswordToken { get; init; }

        public string NewPassword { get; init; }
    }
}
