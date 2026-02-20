using GuardianStock.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace GuardianStock.Application.Users.Commands.SendUserCreatedEmail;

public class SendUserCreatedEmailCommandHandler : IRequestHandler<SendUserCreatedEmailCommand>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<SendUserCreatedEmailCommandHandler> _logger;

    public SendUserCreatedEmailCommandHandler(IEmailService emailService, ILogger<SendUserCreatedEmailCommandHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(SendUserCreatedEmailCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to send User Created Email to {Email}", request.Email);

            var result = await _emailService.SendUserCreatedEmailAsync(
                   request.Email,
                   request.FullName,
                   request.Email,
                   request.Password,
                   cancellationToken);

            if (result.IsFailure)
            {
                _logger.LogError("Failed to send User Created Email to {Email}. Error: {ErrorMessage}", request.Email, result.Message);
                throw new Exception($"Failed to send User Created Email: {result.Message}");
            }

            _logger.LogInformation("Job Completed: User Created Email sent successfully to {Email}.", request.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Job Failed: Error occurred while sending User Created Email to {Email}.", request.Email);
            throw;
        }
    }
}
