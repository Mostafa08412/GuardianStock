using GuardianStock.Application.Common.Interfaces;
using MediatR;

namespace GuardianStock.Application.Auth.ForgetPassword
{
    public class SendForgetPasswordEmailHandler : IRequestHandler<SendForgetPasswordEmail>
    {
        private readonly IEmailService _emailService;

        public SendForgetPasswordEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task Handle(SendForgetPasswordEmail request, CancellationToken cancellationToken)
        {
            await _emailService.SendForgetPasswordEmailAsync(
                request.To,
                request.Name,
                request.EmailAddress,
                request.Otp,
                cancellationToken);
        }
    }
}
