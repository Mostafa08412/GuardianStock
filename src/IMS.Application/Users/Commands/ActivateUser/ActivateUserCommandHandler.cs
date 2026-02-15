using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.ActivateUser;

public class ActivateUserCommandHandler : IRequestHandler<ActivateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public ActivateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UnlockUser(request.UserId, cancellationToken);

        if (result.IsFailure)

            return Result.Failure(result.Errors);


        return Result.Success();
    }
}
