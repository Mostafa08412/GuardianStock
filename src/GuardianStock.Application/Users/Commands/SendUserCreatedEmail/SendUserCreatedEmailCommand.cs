using MediatR;

namespace GuardianStock.Application.Users.Commands.SendUserCreatedEmail;

//NOTE: This command is triggered by a background job.
public record SendUserCreatedEmailCommand : IRequest
{
    public string FullName { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}
