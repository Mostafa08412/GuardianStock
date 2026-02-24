using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Auth.SendEmail
{
    public class SendEmailCommand : IRequest<Result>
    {
        public string To { get; init; }
        public string Subject { get; init; }
        public string Body { get; init; }

    }
}
