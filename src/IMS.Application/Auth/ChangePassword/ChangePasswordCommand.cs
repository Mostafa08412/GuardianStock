using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.ChangePassword
{
    public record ChangePasswordCommand : IRequest<Result>
    {
        public string CurrentPassword { get; init; }

        public string NewPassword { get; init; }

        public string ConfirmNewPassword { get; init; }

    }
}
