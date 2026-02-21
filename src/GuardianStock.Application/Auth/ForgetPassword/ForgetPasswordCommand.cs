using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.ForgetPassword
{
    public record ForgetPasswordCommand : IRequest<Result>
    {
        public string EmailAddress { get; init; }
    }
}
