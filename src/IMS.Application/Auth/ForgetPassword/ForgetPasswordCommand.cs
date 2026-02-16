using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.ForgetPassword
{
    public record ForgetPasswordCommand : IRequest<Result>
    {
        public string EmailAddress { get; init; }
    }
}
