using IMS.Application.Common.Interfaces;
using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Auth.SendEmail
{
    public class SendEmailCommand : IRequest<Result>
    {
        public string To { get; init; }
        public string Subject { get; init; }
        public string Body { get; init; }

    }


    public class SendEmailCommandHandler : IRequestHandler<SendEmailCommand, Result>
    {
        private readonly IEmailService _emailService;

        public SendEmailCommandHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task<Result> Handle(SendEmailCommand request, CancellationToken cancellationToken)
        {
            await _emailService.SendEmailAsync(request.To, request.Subject, request.Body);

            return Result.Success();
        }
    }
}
