using GuardianStock.Application.Common.Interfaces;
using GuardianStock.Domain.Core.Primitives.Result;
using GuardianStock.Application.Users.Commands.SendUserCreatedEmail;
using MediatR;
using System.Security.Cryptography;

namespace GuardianStock.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    private readonly IBackgroundJobWorker _backgroundJobWorker;

    public CreateUserCommandHandler(IIdentityService identityService, IBackgroundJobWorker backgroundJobWorker)
    {
        _identityService = identityService;
        _backgroundJobWorker = backgroundJobWorker;
    }

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {


        var generatedPassword = Convert.ToBase64String(RandomNumberGenerator.GetBytes(12));

        var result = await _identityService.CreateUserAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            generatedPassword,
            cancellationToken,
            request.Role);

        if (result.IsFailure)
            return Result.Failure(result.Errors);

        var command = new SendUserCreatedEmailCommand
        {
            Email = request.Email,
            FullName = $"{request.FirstName} {request.LastName}",
            Password = generatedPassword

        };

        _backgroundJobWorker.EnqueueSendUserCreatedEmailJob(command);

        return Result.Success();
    }
}
