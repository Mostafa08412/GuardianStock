using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.ChangePassword
{
    public record ChangePasswordCommand : IRequest<Result>
    {
        public string CurrentPassword { get; init; }

        public string NewPassword { get; init; }

        public string ConfirmNewPassword { get; init; }

    }
}
