using IMS.Domain.Core.Primitives.Result;
using MediatR;

namespace IMS.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IIdentityService _identityService;

    public UpdateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.UpdateUserAsync(
            request.UserId,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Role,
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.Errors.Any(e => e.Code == "Identity.UserNotFoundById"))
                return Result.Failure(ApplicationErrors.UserErrors.NotFound(request.UserId));

            if (result.Errors.Any(e => e.Code == "Identity.EmailAlreadyExists"))
                return Result.Failure(ApplicationErrors.UserErrors.EmailAlreadyExists(request.Email));

            // For other errors, wrap them
            return Result.Failure(result.Errors);
        }

        return Result.Success();
    }
}
