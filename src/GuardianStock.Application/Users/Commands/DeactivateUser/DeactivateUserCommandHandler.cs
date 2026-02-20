using GuardianStock.Domain.Core.Primitives.Result;
using MediatR;

namespace GuardianStock.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public DeactivateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LockUserAsync(Guid.Parse(request.UserId), cancellationToken);

        if (result.IsFailure)

            return Result.Failure(result.Errors);


        return Result.Success();
    }
}
